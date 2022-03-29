<?php

	//unity import
	$user = $_POST['Input_user'];
	$pass = $_POST['Input_pass'];
	$nick = $_POST['Input_nick'];

	// mysql의 아이디와 비밀번호를 입력해 준다.
	$con = mysqli_connect("localhost", "empyreum0711", "tjswnd683537*", "empyreum0711");
	//내 MySQL(DB)에 접속하기 위한 "empyreum0711"<-ID, "xxxxxxxx"<-패스워드, "empyreum0711"<-DB이름 접속시도
	if(!$con)
		die('Could not Connect:' . mysql_error());  //연결이 실패했을 경우 MySQL을 닫아주겠다는 뜻

	$check = mysqli_query($con, "SELECT * FROM SSDB WHERE `user`='".$user."'");  //SSDB라는 테이블에서 내가 입력한 ID값을 찾겠다.

	// mysql_num_rows() 함수는 데이터베이스에서 쿼리를 보내서 나온 레코드의 개수를 알아낼때 쓰임.
	// 즉 0이 나왔다는 뜻은 내가 찾는 ID값이 없다는 것이다.

	$numrows = mysqli_num_rows($check);
	if ($numrows == 0)
	{
		//정보를 삽입해주는 쿼리문.
		$Result = mysqli_query($con, "INSERT INTO SSDB (`user`, `pass`, `nick_name`) VALUES ('".$user."', '".$pass."', '".$nick."');" ); 
		
		if($Result)
			die("Create Success. \n");
		else
			die("Create error. \n");
	}
	else 
	{
		die("ID does exist. \n");
	}

?>