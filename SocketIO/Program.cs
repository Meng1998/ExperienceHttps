using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketIO
{
    class Program
    {
        static void Main(string[] args)
        {
            socketIO();
            Console.ReadKey();
        }

        public static void socketIO()
        {
            String url = $"http://172.30.248.113:7001?token={Yrtoken()}";
            Console.WriteLine("token:"+ url);
            var socket = IO.Socket(url);


            socket.On("connect", (data) =>      //监听事件
            {
                Console.WriteLine("链接成功");
                var b = (JObject)data;  //将数据转一下
                                        //Console.WriteLine(b['name']);
                Console.WriteLine(data);
                socket.Disconnect();      //退出链接

            });

            socket.On("res", (data) =>      //监听事件
            {
                Console.WriteLine("链接成功");
                var b = (JObject)data;  //将数据转一下
                                        //Console.WriteLine(b['name']);
                Console.WriteLine(data);
                socket.Disconnect();      //退出链接

            });
            //socket.On(Socket.EVENT_CONNECT, () =>       //监听链接
            //{
            //    Console.WriteLine("链接成功");
            //    socket.Emit("pos", "440982");     //发送消息，前面是事件后面是时间  注意：发消息要保证链接是通的，如果链接不通就发数据，再链接上就发不出去

            //    socket.On("pos", (data) =>      //监听事件
            //    {
            //        var b = (JObject)data;  //将数据转一下
            //        //Console.WriteLine(b['name']);
            //        Console.WriteLine(data);
            //        socket.Disconnect();      //退出链接

            //    });
               
            //});
            socket.Connect();   //链接
            //socket.Emit("pos", "354218,430466,430511");
            socket.On(Socket.EVENT_RECONNECT_ATTEMPT, (data) => {
                Console.WriteLine(data);
            });
            socket.On(Socket.EVENT_RECONNECT_FAILED, (data) => {
                Console.WriteLine(data);
            });
            socket.On(Socket.EVENT_RECONNECT_ERROR, (data) => {
                Console.WriteLine(data);
            });
            socket.On(Socket.EVENT_RECONNECT, (data) => {
                Console.WriteLine(data);
            }); 
            socket.On(Socket.EVENT_RECONNECTING, (data) => {
                Console.WriteLine(data);
            });
            socket.On(Socket.EVENT_CONNECT_ERROR, (data) => {
                Console.WriteLine(data);
            });
            socket.On(Socket.EVENT_MESSAGE, (data) => {
                Console.WriteLine(data);
            });
            socket.On(Socket.EVENT_ERROR, (data) => {
                Console.WriteLine(data);
               // socket.Close();
            });
            socket.On(Socket.EVENT_DISCONNECT, () => {
                Console.WriteLine("推出");
                socket.Close();
            });
            socket.On(Socket.EVENT_CONNECT_TIMEOUT, () => {
                Console.WriteLine("推出");
                socket.Close();
            });

            //socket.On(Socket.EVENT_CONNECT, () =>
            //{
            //    //socket.Emit("hi");
            //    socket.Emit("pos", "354218,430466,430511");
            //    Console.WriteLine("hi");
            //});

            //socket.On("connect", (data) =>
            //{
            //    socket.Emit("pos", "354218,430466,430511");
            //    Console.WriteLine("连接：" + data);
            //});

            //socket.On("retransData", (data) =>
            //{
            //    Console.WriteLine("change" + data);
            //});
        }

        public static String Yrtoken()
        {
            try
            {
                var client = new RestClient("http://172.30.248.113:7001/api/login");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", "{\r\n\t \"username\":\"map3d\",\r\n     \"password\":\"123456\"\r\n}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                JObject msg = JObject.Parse(response.Content);
                return msg["data"]["token"].ToString();

            }
            catch (Exception)
            {

                return "";
            }
        }
    }
}
