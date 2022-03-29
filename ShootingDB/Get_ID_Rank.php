<?php
 
    //unity import
    $user = $_POST['Input_user'];
 
    // mysql의 아이디와 비밀번호를 입력해 준다.
    $con = mysqli_connect("localhost", "empyreum0711", "tjswnd683537*", "empyreum0711");
    //내 MySQL(DB)에 접속하기 위한 "seonet"<-ID, "xxxxxxxx"<-패스워드, "loewenherz"<-DB이름 접속시도
 
    if(!$con)
        die('Could not Connect:' . mysqli_error());  //연결이 실패했을 경우 MySQL을 닫아주겠다는 뜻
 
    $check = mysqli_query($con, "SELECT * FROM SSDB WHERE `user`='".$user."'");  //SSDB라는 테이블에서 내가 입력한 ID값을 찾겠다.
 
    $isSuccess = true;
 
    $numrows = mysqli_num_rows($check);
    if ($numrows == 0)
    {
        die("ID does not exist. \n");
        $isSuccess = false;
    }
    else 
    {
        $JSONBUFF = array(); 
 
        //---------------------------- 10위안 리스트 보내기...
                $sqlList = mysqli_query($con, "SELECT * FROM SSDB ORDER BY best_score DESC LIMIT 0, 10");
        //0에서부터 10명까지만...오름차순(ASC)dlsk soflacktns(DESC)
        //* <-- 해당 행의 모든 컬럼(COLUMN)을 가져오라는 뜻 (특정 컬럼(COLUMN)들만 선택적으로 가져올 수 있다. 쉼표로 구분해서...)
    
        $rowsCount = mysqli_num_rows($sqlList);
        if ($rowsCount == 0)
        {
            die("List does not exist. \n");
            $isSuccess = false;
        }
        else 
        {
            $RowDatas = array();
            $Return   = array();
 
            for($aa = 0; $aa < $rowsCount; $aa++)
            {
                $a_row = mysqli_fetch_array($sqlList);       //행 정보 가져오기
                if($a_row != false)
                {
                    //JSON 생성을 위한 변수        
                    $RowDatas['user_id']   = urlencode($a_row['user']);      //한글 포함된 경우    
                    $RowDatas['nick_name'] = urlencode($a_row['nick_name']); //한글 포함된 경우
                    $RowDatas['best_score'] = $a_row['best_score'];
                    $RowDatas['mypoint'] = $a_row['mypoint'];

                    array_push($Return, $RowDatas); //JSON 데이터 생성을 위한 배열에 레코드 값 추가
 
                }//if($a_row != false)
            }//for($aa = 0; $aa < $rowsCount; $aa++)
 
            $JSONBUFF['RkList'] = $Return;   //배열 이름에 배열에 넣기
            //https://cinrueom.tistory.com/6  참고함
 
        }//else         
        //---------------------------- 10위안 리스트 보내기...
 
        //---------------------------- 자신의 랭킹 순위 보내기...
        //그룹화하여 데이터 조회 (GROUP BY) https://extbrain.tistory.com/56
 
        //http://cremazer.blogspot.com/2013/09/mysql-rownum.html : 참고함(1번방법)
        //https://wedul.site/434  //https://link2me.tistory.com/536  //https://lightblog.tistory.com/190 //<--장단점
        //변수는 앞에 @을 붙인다.
        //변수에 값을 할당시 set, select로 할 수 있다. 할당시에는 := 로 한다. 
        mysqli_query($con, "SET @curRank := 0"); //(MY SQL 내에서 사용할 변수 선언법) 변수 사용은 새션내에서만 유효합니다.
        $check = mysqli_query($con, "SELECT user, rank 
                                FROM (SELECT user, 
                                    @curRank := @curRank + 1 as rank 
                                    FROM SSDB 
                        ORDER BY best_score DESC) as CNT 
                              WHERE `user`='".$user."'");
 
        //as 는 변수의 별칭을 만들어서 사용하겠다는 뜻(형변환과 비슷)              
        // https://recoveryman.tistory.com/172
 
        //https://www.bloger.kr/51 : 참고함(2번방법)
        //$check = mysql_query("SELECT user, rank FROM (SELECT user, @curRank := @curRank + 1 as rank FROM SSDB, (SELECT @curRank := 0) t ORDER BY best_score DESC) as CNT WHERE `user`='".$user."'");
 
        //$check = mysql_query("SELECT (SELECT COUNT(*) FROM SSDB WHERE best_score >= sp.best_score) as rank FROM SSDB as sp WHERE `user`='".$user."'"); //<--이건 중복된 등수까지 카운팅해 버림
 
        $numrows = mysqli_num_rows($check);
        if ($numrows == 0)
        {
            die("Ranking search failed for ID. \n");
            $isSuccess = false;
        }
        else 
        {
            if($row = mysqli_fetch_assoc($check))
            {      
                //JSON 파일 생성
                $JSONBUFF['my_rank']   = $row['rank'];   
                header("Content-type:application/json");
                echo urldecode(json_encode($JSONBUFF)); //한글 포함된 경우
                echo ("\n");
            }//if($row = mysql_fetch_assoc($check))
 
        }//else 
 
        //---------------------------- 자신의 랭킹 순위 보내기...
        if (is_bool($isSuccess)) {
            echo ("Get_score_list_Success~");
        }
    }//else 
 
    mysqli_close($con);
 
?>