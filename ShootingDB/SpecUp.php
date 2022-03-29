<?php

//unity import
$user = $_POST['Input_user'];
$hp = $_POST['Input_hp'];
$life = $_POST['Input_life'];
$damage = $_POST['Input_damage'];

// mysql의 아이디와 비밀번호를 입력해 준다.
$con = mysqli_connect("localhost", "empyreum0711", "tjswnd683537*", "empyreum0711");
if(!$con)
	die('Could not Connect:' . mysqli_connect_error());  //연결이 실패했을 경우 이 스크립트를 닫아주겠다는 뜻 //mysql_error() -> mysqli_connect_error()

$check = mysqli_query($con, "SELECT user FROM SSDB WHERE user='".$user."'");  //SSDB라는 테이블에서 내가 입력한 ID값을 찾겠다.

// mysql_num_rows() 함수는 데이터베이스에서 쿼리를 보내서 나온 레코드의 개수를 알아낼때 쓰임.
// 즉 0이 나왔다는 뜻은 내가 찾는 ID값이 없다는 것이다.

$numrows = mysqli_num_rows($check);
if ($numrows == 0)
{
	die("ID does not exist. \n");
}
else 
{

	if($row = mysqli_fetch_assoc($check)) //user 이름에 해당하는 행을 찾아준다.
	{	
 		mysqli_query($con, "UPDATE `SSDB` SET `myhp`= '".$hp."' WHERE `user`= '".$user."' ");  //`user` 를 찾아서 `myhp`= '$hp'로 변경하라는 뜻 
 		mysqli_query($con, "UPDATE `SSDB` SET `mylife`= '".$life."' WHERE `user`= '".$user."' ");  //`info` 를 찾아서'$life'로 변경하라는 뜻 
		mysqli_query($con, "UPDATE `SSDB` SET `mydamage`= '".$damage."' WHERE `user`= '".$user."' ");  //`user` 를 찾아서 `mydamage`= '$damage'로 변경하라는 뜻 
		echo ("UpDateSuccess~");
	}
}

mysqli_close($con);

?>