<?php

include ("./db.php");

$request = $_SERVER['REQUEST_METHOD'];

switch($request){
    case "GET":
        echo json_encode(
            array(
                'users' => getUsers()
            )
        );
        break;
    case "POST":
        if(!empty($_POST["username"]) && !empty($_POST["password"])){
            if(!userExists($_POST["username"])){
                addUser($_POST["username"], $_POST["password"], 0, 1, 0);
                echo json_encode(
                    array(
                        'error' => 0,
                        'message' => "Registration was successful!"
                    ));
            }
            else{
                echo json_encode(
                    array(
                        'error' => 1,
                        'message' => "Username already exists!"
                    ));
            }
        }
        else{
            echo json_encode(
                array(
                    'error' => 1,
                    'message' => "Bad args!"
                ));
        }
        break;
        

    case "PUT":
        $content = file_get_contents('php://input');
        $data = json_decode($content, true);

        if(!empty($data["id"]) && !empty($data["username"]) && !empty($data["balance"])){
            if(is_int(intval($data["balance"]))){
                updateUser($data["id"], $data["username"], $data["balance"]);
                echo json_encode(
                    array(
                        'error' => 0,
                        'message' => "Balance updated succesfully!"
                    )
                );
            }
            else{
                echo json_encode(
                    array(
                        'error' => 1,
                        'message' => "Please enter an integer!"
                    )
                );
            }
            
        }
        else{
            echo json_encode(
                array(
                    'error' => 1,
                    'message' => "Bad args!"
                )
            );
        }
        break;


    case "DELETE":
        $content = file_get_contents('php://input');
        $data = json_decode($content, true);

        if(!empty($data["id"]) && !empty($data["username"])){
            if(userExists($data["username"])){
                deleteUser($data["id"]);
            }
            else{
                echo "User does not exist.";
            }
        }
        else{
            echo "Bad args.";
        }

        break;
    default:
        echo "Bad request.";
        break;
}

function getUsers(){
    global $connect;

    $result = $connect -> query("SELECT * FROM users WHERE user_type_id = 1;");

    return $result -> fetch_all(MYSQLI_ASSOC);
}


function addUser($username, $password, $balance, $user_type_id, $how_many_wins){
    global $connect;

    $connect -> query("INSERT INTO users (username, password, balance, user_type_id, how_many_wins) VALUES ('$username', MD5('$password'), '$balance', '$user_type_id', '$how_many_wins');");
}

function updateUser($id, $username, $balance){
    global $connect;

    $balance = intval($balance);

    $connect -> query("UPDATE users SET balance = balance + '$balance' WHERE username = '$username' AND id = '$id';");
    
}

function deleteUser($id){
    global $connect;

    $connect -> query("DELETE FROM users WHERE id='$id';");
}

function userExists($username){
    global $connect;

    $result = count($connect -> query("SELECT * FROM users WHERE username = '$username';")->fetch_all(MYSQLI_ASSOC));

    return $result > 0;
}