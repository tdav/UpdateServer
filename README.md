
# Error hosting http2
https://github.com/dotnet/aspnetcore/issues/23068


# Code-first gRPC
https://github.com/protobuf-net/protobuf-net.Grpc
https://docs.microsoft.com/en-us/aspnet/core/grpc/code-first?view=aspnetcore-5.0


# Grpc.AspNetCore.Web
https://devblogs.microsoft.com/aspnet/grpc-web-for-net-now-available/
https://azure.github.io/AppService/2021/03/15/How-to-use-gRPC-Web-with-Blazor-WebAssembly-on-App-Service.html

https://github.com/grpc/grpc-dotnet/tree/master/examples#browser


# Dart protoc generated GrpcServiceClient TodoWorld Example
https://docs.servicestack.net/grpc-dart

#Install x dotnet tool:
$ dotnet tool install --global x 
x proto-dart c:\Downloads\eProtocol.proto




    FIREWALL
        firewall-cmd --zone=public --add-port=25002/tcp --permanent
	    firewall-cmd --reload

        firewall-cmd --list-all   # get open port list



 nano /etc/systemd/system/wa_update_manage_server.service
  

[Unit]
Description=UpdateManage Server (Main)

[Service]
WorkingDirectory=/var/www/update_manage
ExecStart=/var/www/update_manage/UniUpdateManage --urls=http://0.0.0.0:50000/
Restart=always

# Restart service after 10 seconds if the dotnet service crashes:
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=quote_server

User=root
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target




