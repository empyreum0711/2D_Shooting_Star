<?php
	//unity import
	$user = $_POST['Input_user'];
	$pass = $_POST['Input_pass'];

	// mysql의 아이디와 비밀번호를 입력해 준다.
	$con = mysqli_connect("localhost", "empyreum0711", "tjswnd683537*", "empyreum0711");
	//내 MySQL(DB)에 접속하기 위한 "empyreum0711"<-ID, "xxxxxxxx"<-패스워드, "empyreum0711"<-DB이름 접속시도

       	if(!$con)
 		die('Could not Connect:' . mysql_error());  //연결이 실패했을 경우 MySQL을 닫아주겠다는 뜻

	mysqli_query($con, "set session character_set_connection=utf8;");
	mysqli_query($con, "set session character_set_results=utf8;");
	mysqli_query($con, "set session character_set_client=utf8;");

	$check = mysqli_query($con, "SELECT * FROM SSDB WHERE `user`='".$user."'");  //SSDB라는 테이블에서 내가 입력한 ID값을 찾겠다.
	$numrows = mysqli_num_rows($check);
	if ($numrows == 0)
	{
 		die("ID does not exist. \n");
	}
	else 
	{
	 	if($row = mysqli_fetch_assoc($check)) //user 이름에 해당하는 행을 모두 찾아준다.
	 	{
  			if($pass == $row['pass'])
			{
				//JSON 생성을 위한 변수
				$RowDatas = array();
				$RowDatas['nick_name'] = urlencode($row['nick_name']); //한글 포함된 경우
				$RowDatas['best_score'] = $row['best_score'];
				$RowDatas['mypoint'] = $row['mypoint'];
				$RowDatas['myhp'] = $row['myhp'];
				$RowDatas['mylife'] = $row['mylife'];
				$RowDatas['mydamage'] = $row['mydamage'];
				$RowDatas['item_list'] = urlencode($row['info']);
				header("Content-type:application/json");
				//JSON 파일 생성	
				$output = json_encode($RowDatas); 

				// 출력
    				echo urldecode($output); //한글 포함된 경우
				echo ("\n");
				die("Login-Success!!");
			}
			else 
				die("Pass does not Match. \n");
		 }
	}
?>