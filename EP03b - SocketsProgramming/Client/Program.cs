﻿using System;
using System.Net;
using System.Threading.Tasks;

using Shared;
using Shared.Xml;

namespace Client
{


    class Program
    {


        static XmlClientChannel Channel = new XmlClientChannel();

        static async Task Main( string[ ] args )
        {

            Console.WriteLine( "Press Enter to Connect" );
            Console.ReadLine( );

            var messageDispatcher = new XDocumentMessageDispatcher();
            messageDispatcher.Register<HeartBeatResponseMessage>( MessageHandler.HandleMessage );
            messageDispatcher.Register<SubmitBasketResponse>( MessageHandler.HandleMessage );


            var endpoint = new IPEndPoint(IPAddress.Loopback, 9000);
            Channel.OnMessage( messageDispatcher.DispatchAsync );
            await Channel.ConnectAsync( endpoint ).ConfigureAwait( false );

            _ = Task.Run( HBLoop );

            var basket = new SubmitBasketRequest {
                Id = "TXN0007",
                POSData = new POSData{ Id = "POS001 "}
            };
            await Channel.SendAsync( basket ).ConfigureAwait( false );

            Console.ReadLine( );
        }

        static async Task HBLoop( )
        {
            while ( true )
            {
                var hbMessage = new HeartBeatRequestMessage {
                    Id = "♥♥HB♥♥",
                    POSData = new POSData{ Id = "POS001 "}
                };
                await Channel.SendAsync( hbMessage ).ConfigureAwait( false );
                await Task.Delay( 10 * 1000 );
            }
        }
    }
}
