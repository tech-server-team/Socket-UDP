const { Console } = require('console');
const dgram = require('dgram');
const server = dgram.createSocket('udp4');

const { v4: uuidv4 } = require('uuid');

const JsonInit = require('./JsonInit.js');

//=====================================
const Port = 6060; // サーバーport定義
const Max_Connection = 4; //最大接続数

//クライアント情報保存
var clients = [];
//=====================================

server.on('error', (err) => {
  console.log(`server error:\n${err.stack}`);
  server.close();
});

//senderInfo = dataを送ったクライアント情報
server.on('message', (msg, senderInfo) => {
  evectHandler(msg, senderInfo)
});

server.on('listening', () => {
  const address = server.address();
  console.log(`server listening on ${address.address}:${address.port}`);
});

//Port番号設定
server.bind(Port);

//=======================
//Jsonデータを解析してEvect処理
//=======================
function evectHandler(msg, senderInfo) {

  try {

    var data = JSON.parse(msg);

    console.log('Messages received ' + msg)
    var thisPlayerId = uuidv4();

    switch (data.type) {

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

      case 'chat':

        break;

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

        console.log("client count : " + Object.keys(clients).length);
        break;

      default:

        break;
    }
  }

  catch (error) {
    console.log(error);
  }
}

//clients Clear
function Clients_Clear(){
  for (let key in clients){
    console.log("disconnection : " + key);
    delete clients[key];
  }
}

//JsonをStringで変換してクライアントへ送信
function Emit(JsonObj, senderInfo) {

  var msg = JSON.stringify(JsonObj);
  server.send(msg, senderInfo.port, senderInfo.address, () => {
    //console.log(`Message sent to ${senderInfo.address}:${senderInfo.port} = ${msg}`)
  })
}

//全クライアントへ送信
function Broadcast(JsonObj, clients, senderInfo) {

  var msg = JSON.stringify(JsonObj);
  for (let index in clients) {

    if (clients[index].port === senderInfo.port)
      continue;

    Emit(JsonObj, clients[index])
  }
}
