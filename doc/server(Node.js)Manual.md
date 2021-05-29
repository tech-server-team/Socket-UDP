# Node.jsインストール
https://nodejs.org/en/

# Server実行

## ターミナルコメント
```bash
cd 自分のパース/UDPSocketManual/Sample-nodejs-udp
node index.js
```

## サーバー実行成功
```bash
server listening on 0.0.0.0:6060
```

# ソースコード説明

## 変数説明
```js
//index.js
//現在、
clients[] 

```

## クライアントからdata受信処理
```js
//index.js
//=======================
//Jsonデータを解析してEvect処理
//=======================
function evectHandler(msg, senderInfo) {

  try {

    var data = JSON.parse(msg);

    console.log('Messages received ' + msg)
    var thisPlayerId = uuidv4();

    switch (data.type) {

    //========================================
    //クライアントid発行
      case 'init':

        //最大接続数がマックスになったら無視する。
        if(Object.keys(clients).length >= Max_Connection){
          return;
        }

        var thisPlayerId = uuidv4();

        //===========
        var JsonObj = data;
        JsonObj.type = 'init';
        JsonObj.uuid = thisPlayerId;
        Emit(JsonObj, senderInfo);
        //===========

        //===========
        JsonObj.uuid = thisPlayerId;
        JsonObj.type = 'init';
        Broadcast(JsonObj, clients, senderInfo);

        for (let index in clients) {
          var dataObj = data;
          dataObj.uuid = index;
          Emit(dataObj, senderInfo);
        }

        clients[thisPlayerId] = senderInfo
        console.log("client count : " + Object.keys(clients).length);
        console.log(senderInfo);
        //===========
        break;
    //========================================


      case 'chat':

        break;
    
    //========================================
    //向こうのクライアントが切断したら
      case 'disconnect':
        
        for (let index in clients) {
          
          if(index === data.uuid) { 
            delete clients[index];
            var jsonobj = data;
            jsonobj.type = "client_Reload";
            jsonobj.uuid = data.uuid;
            Broadcast(jsonobj, clients, senderInfo);
            break;
          }
        }
    //========================================
        console.log("client count : " + Object.keys(clients).length);
        break;
    //========================================
    //ここで処理コード追加
    //case :
    //  break;
    //========================================
      default:

        break;
    }
  }

  catch (error) {
    console.log(error);
  }
}
```

## Base Code

https://nodejs.org/api/dgram.html
```js
const dgram = require('dgram');
const server = dgram.createSocket('udp4');

server.on('error', (err) => {
  console.log(`server error:\n${err.stack}`);
  server.close();
});

server.on('message', (msg, rinfo) => {
  console.log(`server got: ${msg} from ${rinfo.address}:${rinfo.port}`);
});

server.on('listening', () => {
  const address = server.address();
  console.log(`server listening ${address.address}:${address.port}`);
});

server.bind(41234);
// Prints: server listening 0.0.0.0:41234
```

## クライアントへData送信
```js

var dataObj = data;
dataObj.type = 'init';
dataObj.msg = 'hello world in server';

//Emit(送信データ、送りたいクライアント)
Emit(dataObj, senderInfo);

//senderInfoを除いてサーバーに接続している全クライアントにへjsonobjを送信する。
Broadcast(jsonobj, clients, senderInfo);
```