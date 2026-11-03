package main

import (
	"context"
	"encoding/json"
	"log"
	"time"

	"github.com/downloadNode/message"
	"github.com/downloadNode/model"
	rmq "github.com/rabbitmq/rabbitmq-amqp-go-client/pkg/rabbitmqamqp"
)

const (
	DownloadOrdersExchange    = "download_orders"
	DownloadOrdersAddQueue    = "download_orders_add"
	DownloadOrdersUpdateQueue = "download_orders_update"
)

func main() {
	ctx := context.Background()

	connection := message.CreateConnection(ctx, false)
	defer connection.Close(ctx)

	consumer := message.GetConsumer(ctx, connection, DownloadOrdersAddQueue)

	log.Printf(" [*] Waiting for messages. To exit press CTRL+C")

	message.Consume(ctx, consumer, func(downloadRequest model.DownloadRequest, delivery rmq.IDeliveryContext) {
		log.Printf("Download Id: %d was received!", downloadRequest.Download.Id)

		downloadRequest.Download.StartedAt = time.Now().Format("2006-01-02T15:04:05")
		downloadRequest.Download.LastTimeAccessed = time.Now().Format("2006-01-02T15:04:05")
		downloadRequest.Download.Status = 1

		publisher := message.CreatePublish(ctx, connection, DownloadOrdersExchange, "update")

		if publisher == nil {
			log.Printf("Requeue Download Id: %d", downloadRequest.Download.Id)
			delivery.Requeue(ctx)
			return
		}

		downloadRequestJson, err := json.Marshal(downloadRequest)
		if err != nil {
			log.Printf("Download Id: %d was a invalid Download Request! %s", downloadRequest.Download.Id, err)
			log.Printf("Requeue Download Id: %d", downloadRequest.Download.Id)
			delivery.Requeue(ctx)
			return
		}

		message.PublishMessage(publisher, ctx, string(downloadRequestJson))

		err = delivery.Accept(ctx)

		if err != nil {
			log.Printf("Download Id: %d was not accepted! %s", downloadRequest.Download.Id, err)
			return
		}

	})

	consumer.Close(ctx)
	connection.Close(ctx)

	log.Println("Closing node!")
}
