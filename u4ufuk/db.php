<?php

    $host = "localhost";
    $port = 3306;
    $socket = "";
    $user = "root";
    $password = "";
    $dbname = "u4ufuk";

    $connect = new mysqli($host, $user, $password, $dbname, $port, $socket)
        or die("Connection has failed: " . mysqli_connect_error());
    
    if($connect -> errno >  0){
        printf("Connection has failed:" . mysqli_connect_error());
        exit();
    }
