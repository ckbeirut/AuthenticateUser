    Public Function authenticateUser(ByVal usrName As String, ByVal usrPswd As String) As Boolean
        Try
            Dim loginUri As New Uri("https://website.com/Account.aspx/Logon?app=name")            
            Dim ssoRequest As HttpWebRequest = TryCast(WebRequest.Create(loginUri), HttpWebRequest)

            'Escape special characters such as + => %2B
            usrPswd = HttpUtility.UrlEncode(usrPswd)
            Dim loginstr As String = String.Format("username={0}&password={1}", usrName, usrPswd)
            'contains form values
            Dim reqBytes As Byte() = Encoding.ASCII.GetBytes(loginstr)
            Dim dataStream As Stream = Nothing
            Dim ssoResponse As HttpWebResponse = Nothing
            Dim cookieJar As New CookieContainer()

            'cookies for request
            Dim ssoCookieDish As CookieCollection = Nothing
            'cookies returned by SSO

            'set parameters for request
            ssoRequest.Method = "POST"
            ssoRequest.ContentType = "application/x-www-form-urlencoded"
            ssoRequest.CookieContainer = cookieJar
            ssoRequest.ContentLength = reqBytes.Length
            ssoRequest.Timeout = 30000

            'write form values to submit
            dataStream = ssoRequest.GetRequestStream()
            dataStream.Write(reqBytes, 0, reqBytes.Length)
            dataStream.Close()

            'send request, returned streams do not interest us 
            ssoResponse = TryCast(ssoRequest.GetResponse(), HttpWebResponse)

            'MCSEC cookie will be empty if login is not successful
            ssoCookieDish = cookieJar.GetCookies(loginUri)

            If ssoCookieDish("MCSEC") Is Nothing Then
                Return False
            Else
                Return True
            End If
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
    End Function
