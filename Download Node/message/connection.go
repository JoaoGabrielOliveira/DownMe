package message

import (
	"context"
	"log"
	"time"

	rmq "github.com/rabbitmq/rabbitmq-amqp-go-client/pkg/rabbitmqamqp"
)

const BROKER_URI = "amqp://go-consumer:go-consumer@localhost:5672/%2Fmain"

func CreateConnection(ctx context.Context, applyRetry bool) *rmq.AmqpConnection {
	env := rmq.NewEnvironment(BROKER_URI, nil)

	var connection *rmq.AmqpConnection
	var err error
	var intervalTimeout int = 5

	for {
		connection, err = env.NewConnection(ctx)
		if err != nil && !applyRetry {
			log.Fatalln("Error to create connection!")
		}

		if connection != nil {
			break
		}

		log.SetFlags(log.LstdFlags | log.Lmicroseconds)
		log.Printf("Error to create connection. Trying again in %02d seconds", intervalTimeout)
		time.Sleep(time.Second * time.Duration(intervalTimeout))

		if intervalTimeout <= 40 {
			intervalTimeout *= 2
		}
	}

	return connection
}
