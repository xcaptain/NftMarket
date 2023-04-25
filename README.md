# nft market

从区块链上索引订单，从第三方接口索引元数据，建立自己的nft交易市场

## 架构

核心是一个asp net项目，通过nethereum索引区块，保存到sql server，然后建立一个 microsoft data api builder对外提供服务


### redpanda

使用 redpanda 作为消息队列，用于异步处理，`docker compose up -d` 启动broker，然后就能 `dotnet run` 进行消费

1. https://docs.redpanda.com/docs/get-started/quick-start/?quickstart=docker
2. https://developer.confluent.io/get-started/dotnet/#build-producer

### TODO

- [ ] BlockStorageProcessor 发消息
- [ ] 消费者异步处理消息，解析事件