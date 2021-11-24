#   Установка


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
    
    RestartSec=10
    KillSignal=SIGINT
    SyslogIdentifier=quote_server

    User=root
    Environment=ASPNETCORE_ENVIRONMENT=Production
    Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

    [Install]
    WantedBy=multi-user.target




