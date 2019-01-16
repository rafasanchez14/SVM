﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;

namespace SVM_SA.Util
{
    class QuerySqlite
    {
        public static SQLiteConnection sql_con(bool first=false) {  
            if (first)
            {
                SQLiteConnection.CreateFile(GenericFunction.GetExecutingDirectoryName() + "svm.db3");
            }
            return new SQLiteConnection(@"Data Source ="+ GenericFunction.GetExecutingDirectoryName()+ @"svm.db3;
        Version =3; FailIfMissing=True; Foreign Keys=True;");
        } 


        public static string CreateTableConf => @"
            CREATE TABLE configSVM (
             id INTEGER,
             ip TEXT,
             port TEXT NOT NULL,
             isPrincipal INTEGER,
             conn TEXT
            );
        ";

        public static string UpdateConfIsPrincipal => @"
        UPDATE configSVM
        SET isPrincipal = @Inprincipal
        WHERE ip=@InIp AND 
        port = @InPort ;
        ";

        public static string InsertConf => @"
       
        INSERT INTO configSVM (id,ip,port,isPrincipal,conn) VALUES 
                (1
                ,@InIp
                ,@InPort
                ,@Inprincipal
                ,@InConex);
        ";

        public static string SelectIsPrincipal => @"
        SELECT isPrincipal 
        FROM configSVM
        WHERE ip=@InIp AND 
        port = @InPort ;
        ";
    }
}