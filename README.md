# nft market

从区块链上索引订单，从第三方接口索引元数据，建立自己的nft交易市场

## 架构

核心是一个asp net项目，通过nethereum索引区块，保存到sql server，然后建立一个 microsoft data api builder对外提供服务

