Imports System
Imports System.IO
Imports System.Text
Imports System.Text.Json
Imports System.Text.Json.Serialization
Imports System.Xml
Imports System.Xml.Serialization
Imports System.Globalization

' User language = Galician or Slowak, Enums and the Winter Keyword = English
#Const SlovakVersion = False


Module Program
  Public todo1(20) As String
  Public filled1bis As Integer = 0
  Public todo3 As String = Nothing
  Dim todayurl = Nothing
  Dim timestamp1,timestamp2,timestamp3,timestamp4 As DateTime ' used time = t2-t1 + t4-t3
  Dim nsteps as Integer = 0 ' how many steps in that time ?
  Dim newseeding As Boolean = False

  Sub Main(args As String())
      LoadAData
      If DateTime.Today.DayOfWeek = DayOfWeek.Sunday Then
        Dim inpu As String = "abc"
#If SlovakVersion Then
          If dataForM.TkWeight Then
            Console.Write("Hmotnosť v kg : ")
            inpu = Console.ReadLine()
          End If
          stoerungen2 = janein1("Bola cesta zle využitá")
          If  stoerungen2 Then
              todo3 = "Zistite príčinu rušnej ulice!"
          Else
              Console.WriteLine("ok")
          End If
          newseeding = janein1("Nový výsev")
#Else
          If dataForM.TkWeight Then
            Console.Write("Peso en kg : ")
            inpu = Console.ReadLine()
          End If
          stoerungen2 = janein1("¿Tráfico pesado na estrada")
          If  stoerungen2 Then
              todo3 = "Descubra a causa da estrada transitada"
          Else
              Console.WriteLine("nada")
          End If
          newseeding = janein1("Nova sementeira")
#End if
        If dataForM.TkWeight Then
          Dim style As NumberStyles
          Dim provider As CultureInfo
          style = NumberStyles.AllowDecimalPoint Or NumberStyles.AllowThousands
          provider = New CultureInfo("en-US")
          If Not Decimal.TryParse(inpu.Replace(",","."),style,provider,theWeight) Then  errorExit(inpu,new FormatException)
        End If
      End If
      Console.WriteLine(VbLf)
      deferred.WaterPlants1 = False
      LoadMorningXmlIfExists
      If newseeding Then deferred.SeedingDay = DateTime.ToDay.DayOfYear + 365*((DateTime.ToDay.Year-1) mod 4 ) 
      PlanningQuestions
      SaveMorningXml
      Array.Resize( todo1,filled1bis+1 )
      left = todo1
      r = wForM.TodoPart2
      If Not isHeatingSeason() Then 
          r = Array.FindAll( wForM.TodoPart2, Function(p As String ) IsNothing(p) OrElse Not p.StartsWith("Winter:") )
      End If
      If (DateTime.ToDay.DayOfWeek = 2 OrElse DateTime.ToDay.DayOfWeek = 4 or DateTime.ToDay.DayOfWeek = 0)  AndAlso r( r.Length-1 ).StartsWith("Entscheide") Then
        Dim fnz As String = dirForJS & "zstring.mtxt"
        Try
          r( r.Length-1 ) = File.ReadAllText( fnz ).Trim().Replace(VbLf," ")
          If len(r( r.Length-1 ))>32 Then r( r.Length-1 ) = r( r.Length-1 ).SubString( 0,32 )&".."
        Catch eeee As Exception  
        End Try 
      End If
      timestamp1 = DateTime.Now
      dozweispaltigchecklist( left, r, todayurl ) 
      If wForM.SaveAStringPrompt.Length() >= 2 AndAlso Not Char.IsWhiteSpace(wForM.SaveAStringPrompt(1)) Then
        EnterAndSaveString
      End If
      deferred.WaterPlants1 = False
      SaveMorningXml
      If justTookV > -2 Then LoadOrSaveVitamin( justTookV )
      timestamp4 = DateTime.Now
      If DateTime.Today.DayOfWeek = DayOfWeek.Sunday AndAlso DateTime.Now.Hour >= 4  Then
        UpdateJsonStatist
      End If
      Dim interval As TimeSpan
      If timestamp2 <> Nothing AndAlso timestamp3 <> Nothing Then
        interval = (timestamp2 - timestamp1)+(timestamp4 - timestamp3)
      Else
        interval = timestamp4 - timestamp1
      End If
#If SlovakVersion Then
      Console.Write("Potrebovali ste {0} minút na {1} krokov ", interval.Hours * 60 + interval.Minutes, nsteps )
      If nsteps > 0 Then Console.WriteLine("= {0:N1} minút na krok. ", interval.TotalMinutes/nsteps )
      Console.WriteLine("Program ukončíte stlačením iného tlačidla.")
#Else
      Console.Write("Tardaron {0} minutos en {1} pasos ", interval.Hours * 60 + interval.Minutes, nsteps )
      If nsteps > 0 Then Console.WriteLine("= {0:N1} minutos por paso. ", interval.TotalMinutes/nsteps )
      Console.WriteLine("Prema outra tecla para finalizar o programa.")
#End If
      Dim ee AS String
      Do
        ee = Console.ReadKey(true).KeyChar
      Loop Until Not "lLeEdDrRustUSTpP".Contains(ee)
  End Sub


    
  Dim dataForM as DataForMorningP
  Dim wForM    as WorteForMorningP
  Dim deferred as DeferredFor
  Dim dirForJS = "jsonxmlm/"
  Dim stoerungen2 As Boolean = false
  Dim theWeight As Decimal = -2
  Dim justTookV As Integer = -2
  Dim wash2time As DateTime
  Dim wash2do As Boolean = false

 
  Private Sub LoadAData
#If SlovakVersion Then
    Dim fn = "jsonxmlm/data1_for_morning_slovak.xml"
#Else
    Dim fn = "jsonxmlm/data1_for_morning_galician.xml"
#End If
    Try
      For Teste = 1 To 5
        If not File.Exists(fn) Then 
          fn = "../" & fn
          dirForJS = "../" & dirForJS
        End If
      Next
      Dim stm As FileStream = New FileStream(fn, FileMode.Open)
      Dim xmlSer As XmlSerializer = New XmlSerializer(GetType(DataForMorningP))
      dataForM = CType(xmlSer.Deserialize(stm), DataForMorningP)
      stm.Close()    ' Testprint waere:   Console.WriteLine(dataForM.DayOfLastWalkOrJogging)
      Dim cd = new StringBuilder( Directory.GetCurrentDirectory() )
      Console.Write( cd )
#If SlovakVersion Then
      fn = "jsonxmlm/words_for_morning_slovak.xml"
#Else
      fn = "jsonxmlm/words_for_morning_galician.xml"
#End If
      For Teste = 1 To 5
        If not File.Exists(fn) Then 
          fn = "../" & fn
          Dim lin as Integer =   cd.ToString().LastIndexOf("\")
          If lin = -1 Then lin = cd.ToString().LastIndexOf("/")
          If Teste = 1 AndAlso lin > 4 Then
            cd.Length = lin
            Console.Write( VbCr & cd.ToString() )
          Else
            Console.Write("/..")
          End If
        End if        
      Next
      stm = New FileStream(fn, FileMode.Open)
      xmlSer = New XmlSerializer(GetType(WorteForMorningP))
      wForM = CType(xmlSer.Deserialize(stm), WorteForMorningP)
      stm.Close()    ' Testprint waere:   Console.WriteLine(wForM.VersionOfStruct)
#If SlovakVersion Then
      Console.WriteLine( "/jsonxmlm/*.xml" & " cargado" )
      If dataForM.WhereSundayToSaturday Is Nothing Then Console.WriteLine("Varovanie: WhereSundayToSaturday nie je k dispozícii v daten_fuer_morgen.xml") 
      If dataForM.MorningExerciseModeArray Is Nothing Then Console.WriteLine("Varovanie: MorningExerciseModeArray nie je k dispozícii v daten_fuer_morgen.xml")
      If wForM.URL_ofDay Is Nothing OrElse wForM.URL_ofDay.Length() = 0 Then 
        Console.WriteLine("Varovanie: URL_ofDay nie je k dispozícii v daten_fuer_morgen.xml")
#Else
      Console.WriteLine( "/jsonxmlm/*.xml" & "  sa načítal" )
      If dataForM.WhereSundayToSaturday Is Nothing Then Console.WriteLine("Aviso:Non está presente WhereSundayToSaturday en data1_for_morning_galician.xml") 
      If dataForM.MorningExerciseModeArray Is Nothing Then Console.WriteLine("Aviso: Non está presente MorningExerciseModeArray en data1_for_morning_galician.xml?")
      If wForM.URL_ofDay Is Nothing OrElse wForM.URL_ofDay.Length() = 0 Then 
        Console.WriteLine("Aviso: Non está presente URL_ofDay en data1_for_morning_galician.xml?")
#End if
      Else If wForM.URL_ofDay.Length() Mod 7 = 0 Then
        Array.Resize(wForM.URL_ofDay, wForM.URL_ofDay.Length + 1) ' Should not be the same URL for the same weekday
        wForM.URL_ofDay(wForM.URL_ofDay.Length - 1) = wForM.URL_ofDay( DateTime.Today.DayOfWeek )
      End If
    Catch eee As Exception  
#If SlovakVersion Then
      errorExit(fn & "   Chyba:", eee)
#Else
      errorExit(fn & "   Erro:", eee)
#End if
    End Try 
  End Sub



  
  Private Sub UpdateJsonStatist
    Dim fn As String = dirForJS & "trafficstat.json"
    Dim fnw As String = dirForJS & "wstat.json"
    Dim trafficFileDate As Decimal = 0.0
    Try
      Console.Write(fn & "  ")
      Dim ts As StreetStatist
      If File.Exists(fn) = False Then
        ts = new StreetStatist()
        ts.C = DateTime.Today.Year
        Dim vvv = New List(Of VTupel)()
        ts.MuchTraffic = vvv
      Else
        Dim readText As String = File.ReadAllText(fn)
        ts = JsonSerializer.Deserialize(Of StreetStatist)(readText)
        trafficFileDate = ts.C
      End If
      ts.C = DateTime.Today.Year
      If DateTime.ToDay.Month < 10 Then
        ts.C = ts.C + 0.100D * DateTime.Today.Month
      Else If DateTime.ToDay.Month < 12 Then
        ts.C = ts.C + 0.930D + 0.035D * (DateTime.Today.Month-10)
      Else
        ts.C = ts.C + 1.000D
      End If
      ts.C = ts.C + 0.001D * DateTime.Today.Day
      If ts.C <> trafficFileDate Then
        Dim s1 = New VTupel()
        s1.D = ts.C
        s1.V = 0
        If stoerungen2 Then s1.V = 111
        ts.MuchTraffic.Add(s1)
      End If
      If File.Exists(fn) Then File.Delete(fn)
      Using fs As FileStream = File.Create(fn)
        Dim dw As New StreamWriter(fs)
        Dim jsonString =JsonSerializer.Serialize(ts,GetType(StreetStatist) )
        jsonString = jsonString.Replace( "}", "}" & VbCrLf ) ' Instead Formatting inserting some LF´s
        dw.WriteLine( jsonString )
        dw.Close()
#If SlovakVersion Then
        Console.WriteLine( " uložené.")
#Else
        Console.WriteLine( " gardado.")
#End if
      End Using
      If dataForM.TkWeight Then
        Console.Write(fnw & "  ")
        Dim tsw As String
        tsw=""
        If File.Exists(fnw) Then tsw = File.ReadAllText(fnw) 
        tsw = tsw.Trim()
        If ""=tsw OrElse tsw(0)<>"{"c Then 
          tsw = "{""Y"":" & DateTime.ToDay.Year.ToString() &",""W"":[" & VbCrLf & "[" & tsw
        Else
          Dim lentsw = tsw.Length()
          Dim wFileYear As Integer = 0
          If lentsw>10 Then 
            If Not Integer.TryParse( tsw.SubString(5,4) , wFileYear) Then
              Console.WriteLine( tsw.SubString(5,4) & " - " )
              If 5000 > lentsw Then Console.WriteLine(tsw)
#If SlovakVersion Then
              Console.Write( " - Varovanie týkajúce sa ")
              Console.WriteLine( fnw & ": Leto je napačno oblikovano, obsah bude odstránený - ")
#Else
              Console.Write( " - Aviso sobre ")
              Console.WriteLine( fnw & ": O ano ten un formato incorrecto, eliminaranse os contidos - ")
#End if
              tsw = ""
              lentsw = 0
            End If
          End If
          If lentsw>7 AndAlso tsw(lentsw-1)="}"c Then tsw = tsw.SubString(0,lentsw-3) 
          If lentsw>20 AndAlso wFileYear<>DateTime.ToDay.Year Then tsw = tsw.SubString(0,5) & DateTime.ToDay.Year.ToString() & tsw.SubString(5+4) & "]," & VbCrLf & "[" 
        End If
        Dim begyindex = tsw.LastIndexOf( "["c )
        Dim countInYear=1
        If begyindex > 7 Then
          For Each c As Char In tsw.SubString( begyindex )
            If c = ","c Then 
              countInYear += 1
            End If
          Next
        End If
        If tsw<>"" AndAlso tsw(tsw.Length()-1)<>"["c Then 
          tsw = tsw & ","
          countInYear = countInYear + 1
        End If
        Dim cal As Calendar = new GregorianCalendar()
        Dim weekOfYear = cal.GetWeekOfYear(DateTime.ToDay,CalendarWeekRule.FirstDay,DayOfWeek.Monday)
        Dim lio As Integer = tsw.LastIndexOf( VbLf )
        If lio < 0 Then lio = 0
        If 27*(tsw.Length()-lio)-333 > (theWeight-60)*(theWeight-60) Then tsw = tsw & VbCrLf
        tsw = tsw & theWeight.ToString().Replace(",",".") & "]]}"
        Console.Write( "(#" & countInYear & " / " & weekOfYear & ")" )
        If countInYear <= weekOfYear Then 
          Using fsw As FileStream = File.Create(fnw)
            Dim dww As New StreamWriter(fsw)
            dww.WriteLine( tsw )
            dww.Close()
#If SlovakVersion Then
            Console.WriteLine( "   uložené.")
#Else
            Console.WriteLine( "   gardado.")
#End if
          End Using       
        Else
          Console.WriteLine( " - countInYear > weekOfYear.")
        End If
      End If
    Catch eee As Exception  
#If SlovakVersion Then
      errorExit(fn & " UpdateJsonStatist  Chyba:", eee)
#Else
      errorExit(fn & " UpdateJsonStatist  Erro:", eee)
#End if
    End Try 
  End Sub



  Private Sub SaveMorningXml
    Dim fn As String = dirForJS & "deferredmorning.mxml"
    Try
      Dim serializer As New XmlSerializer(GetType(DeferredFor))
      Dim fs As New FileStream(fn, FileMode.Create)
      Dim writer As New XmlTextWriter(fs, Encoding.UTF8)
      serializer.Serialize(writer, deferred)
      writer.Close()
      Console.Write(  VbCrLf & fn )
#If SlovakVersion Then
      Console.WriteLine( "   uložené." & VbCrLf )
#Else
      Console.WriteLine( "   gardado." & VbCrLf )
#End if
    Catch eee As Exception  
#If SlovakVersion Then
      errorExit(fn & "   Chyba:", eee)
#Else
      errorExit(fn & "   Erro:", eee)
#End if
    End Try 
  End Sub




  Private Sub LoadMorningXmlIfExists
    Dim fn As String = dirForJS & "deferredmorning.mxml"
    If Not File.Exists( fn ) Then Return
    Dim stm As FileStream = Nothing
    Try
      stm  = New FileStream(fn, FileMode.Open)
      Dim xmlSer As XmlSerializer = New XmlSerializer(GetType(DeferredFor))
      deferred = CType(xmlSer.Deserialize(stm), DeferredFor)
      stm.Close()
      Console.Write( " ~ " )
    Catch eee As Exception  
      Dim text1 as String
#If SlovakVersion Then
      text1 = fn & "  TryMorningXml Chyba:"
#Else
      text1 = fn  & "  TryMorningXml Erro:"
#End if
      Console.WriteLine(text1)    
      if eee IsNot Nothing Then Console.WriteLine(eee)
      If stm IsNot Nothing Then stm.Close() 
#If SlovakVersion Then
      Console.WriteLine("Ale pokračujte ďalej")
#Else
      Console.WriteLine("Pero segue adiante")
#End if
    End Try 
  End Sub




  Private Sub EnterAndSaveString
    Dim fn As String = dirForJS & "zstring.mtxt"
    If DateTime.ToDay.DayOfWeek = 2 Then 
      fn = dirForJS & "hdtws.mtxt"
      Console.Write( VbCrLf & VbCrLf & "How did the week start? " )
    Else If DateTime.ToDay.DayOfWeek = 4 Then 
      fn = dirForJS & "bureaucracy-or-looks.mtxt"
#If SlovakVersion Then
      Console.WriteLine(VbCrLf & VbCrLf &"Ako ste prišli s byrokraciou alebo vzhľadom?")
#Else
      Console.WriteLine(VbCrLf & VbCrLf &"Como você vasculhou com burocracia ou o olhar?")
#End if
    Else If DateTime.ToDay.DayOfWeek = 0 Then 
      fn = dirForJS & "repair-or-learn-edv.mtxt"
#If SlovakVersion Then
      Console.WriteLine(VbCrLf & VbCrLf &"Čo ste opravili alebo čo ste programovali alebo ste sa naučili?")
#Else
      Console.WriteLine(VbCrLf & VbCrLf &"Que reparaches ou que programaches ou aprendiches?")
#End if
    Else
      Console.Write( VbCrLf & VbCrLf & wForM.SaveAStringPrompt )
    End If
    If DateTime.ToDay.DayOfWeek = 2-1 OrElse DateTime.ToDay.DayOfWeek = 4-1  OrElse DateTime.ToDay.DayOfWeek = 7-1 Then 
#If SlovakVersion Then
      Console.Write( " (Dva dni) " )
#Else
      Console.Write( " (Dous días) " )
#End if
    End If
    If Not "!?:".Contains( wForM.SaveAStringPrompt(wForM.SaveAStringPrompt.Length()-1)) Then Console.Write( ":" )
    Console.Write( " " )
    Dim input As String
    Try
      input = Console.ReadLine()
      Dim old As String = ""
      If File.Exists(fn) Then
        If DateTime.ToDay.DayOfWeek = 2 or DateTime.ToDay.DayOfWeek = 4 or DateTime.ToDay.DayOfWeek = 0  Then
          Dim hinducalday =  (DateTime.ToDay.Year-2021)*366+DateTime.ToDay.DayOfYear-14
          ' Number the days as Hindu season which begin approximately Mid-Jan/Mar/May...
          old =  File.ReadAllText(fn) & VbCrLF & (hinducalday\61).ToString() & "." & (1+hinducalday mod 61).ToString().PadLeft(2,"0"c) & " : "
        End If 
        File.Delete(fn)
      Else If DateTime.ToDay.DayOfWeek = 2 or DateTime.ToDay.DayOfWeek = 4 or DateTime.ToDay.DayOfWeek = 0  Then
          Dim hinducaldaz =  (DateTime.ToDay.Year-2021)*366+DateTime.ToDay.DayOfYear-14
          old = "Ritu_(Indian_season)" & VbCrLF & (hinducaldaz\61).ToString() & "." & (1+hinducaldaz mod 61).ToString().PadLeft(2,"0"c) & " : "
      End If
      Using fs As FileStream = File.Create(fn)
        Dim dw = New StreamWriter(fs)
        dw.Write( old & input )
        dw.Close()
        Console.Write(  VbCrLf & fn )
#If SlovakVersion Then
        Console.WriteLine( "   uložené.")
#Else
        Console.WriteLine( "   gardado.")
#End if
      End Using
    Catch eee As Exception  
#If SlovakVersion Then
      errorExit(fn & "   Chyba:", eee)
#Else
      errorExit(fn & "   Erro:", eee)
#End if
    End Try 
  End Sub



  Private Sub SaveViewLater( url As String )
    Dim fn As String = dirForJS & "view-later-top-in.json"
    Try
      Dim old As String = VbCrLF & "]"
      If File.Exists(fn) AndAlso FileLen(fn)>9  AndAlso FileLen(fn)<888 Then
        old =  VbCrLF & "," & VbCrLf & File.ReadAllText(fn).SubString(3)
        File.Delete(fn)
      End If
      Using fs As FileStream = File.Create(fn)
        Dim dw = New StreamWriter(fs)
        dw.Write( "[" & VbCrLF & "    """ & url & """" & old )
        dw.Close()
      End Using
    Catch eee As Exception  
#If SlovakVersion Then
      errorExit(fn & "   Chyba:", eee)
#Else
      errorExit(fn & "   Erro:", eee)
#End if
    End Try 
  End Sub



  Function LoadOrSaveVitamin(savenumber As Integer) ' if > -2 then save
    Dim fn As String = dirForJS & "vitamintaken.mtxt"
    Dim numstring As String
    Try
      If savenumber >= -1 Then
        numstring = ",,Z," & savenumber.ToString() 
        If File.Exists(fn) Then File.Delete(fn)
        Using fs As FileStream = File.Create(fn)
          Dim dw = New StreamWriter(fs)
          dw.Write( numstring )
          dw.Close()
        End Using
        Console.Write(  VbCrLf & fn )
#If SlovakVersion Then
        Console.WriteLine( "   uložené.")
#Else
        Console.WriteLine( "   gardado.")
#End if
        Return 7777777
      Else 
        If Not File.Exists(fn) Then Return -9
        numstring = File.ReadAllText( fn )
        If numstring.Length() < 4 Then  Return -9
        Dim u As Integer
        u = Integer.Parse( numstring.Substring(4) )
        Return u
      End If        
    Catch eee As Exception  
#If SlovakVersion Then
      errorExit(fn & "   Chyba:", eee)
#Else
      errorExit(fn & "   Erro:", eee)
#End if
      Return -9
    End Try 
  End Function



  Function isHeatingSeason()
    Dim mon = DateTime.Now.Month
    Dim day = DateTime.Now.Day
    If dataForM.WinterHeatingAbMMDD < dataForM.WinterHeatingBisMDD Then ' Suedhalbkugel
      If dataForM.WinterHeatingAbMMDD\100 < mon AndAlso mon < dataForM.WinterHeatingBisMDD\100 Then Return true
      If dataForM.WinterHeatingAbMMDD\100 = mon Then Return day >= dataForM.WinterHeatingAbMMDD mod 100
      If dataForM.WinterHeatingBisMDD\100  = mon Then Return day <= dataForM.WinterHeatingBisMDD mod 100
      Return False
    Else ' Nordhalbkugel
      If dataForM.WinterHeatingAbMMDD\100 < mon OrElse mon < dataForM.WinterHeatingBisMDD\100 Then Return true
      If dataForM.WinterHeatingAbMMDD\100 = mon Then Return day >= dataForM.WinterHeatingAbMMDD mod 100
      If dataForM.WinterHeatingBisMDD\100  = mon Then Return day <= dataForM.WinterHeatingBisMDD mod 100
      Return False
    End If
  End Function



  Sub PlanningQuestions
    ' Setzt todo1, filled1bis, todayurl
    Dim day146097 As Integer = DateDiff(DateInterval.Day, GlobalConstants.base_sunday , Date.Now)
    day146097 = day146097 mod 146097
    Dim fs as MorningExerciseMode
    fs = PhysicalActivityDays( day146097 , dataForM )
#If SlovakVersion Then
    Console.Write("Dobré ráno. ")
#Else
    Console.Write("Bos días. ")
#End if
    ' MorningExerciseMode auf NoMorningExercise und AlmostAlways reduzieren
    Dim tempquest As String = ""
#If SlovakVersion Then
    If isHeatingSeason() Then tempquest = " alebo je to pod -5 stupňov"
    if MorningExerciseMode.WeatherQuestion = fs AndAlso janein1("Prší" & tempquest) then
#Else
    If isHeatingSeason() Then tempquest = " ou está baixo os -5 graos"
    if MorningExerciseMode.WeatherQuestion = fs AndAlso janein1("¿Chove" & tempquest) then
#End if
      fs = MorningExerciseMode.NoMorningExercise
    Elseif MorningExerciseMode.WeatherQuestion = fs then
      fs = MorningExerciseMode.AlmostAlways
    End if
    Console.WriteLine(VbLf)
    If DateTime.Now.Hour >= 7 AndAlso DateTime.Today.DayOfWeek = DayOfWeek.Monday Then
      filled1bis = filled1bis + 1
#If SlovakVersion Then
      todo1(filled1bis) = "V prípade potreby zapnite umývačku riadu"
#Else
      todo1(filled1bis) = "Se é necesario, un lavaplatos"
#End if
    End If
    Dim tookVit As Integer = LoadOrSaveVitamin(-2) - day146097
    If day146097 < = 44444 AndAlso tookVit > 77777 Then
      tookVit = tookVit - 146097
    Else If day146097 >= 77777 AndAlso tookVit < 44444 Then
      tookVit = tookVit + 146097
    End If
    Dim takeVit As Boolean = tookVit  < -1 
#If SlovakVersion Then
    If tookVit <> -1 Then Console.WriteLine( "       Vitamín prijatý: " & tookVit.ToString() & " deň" & VbCrLf )
#Else
    If tookVit <> -1 Then Console.WriteLine( "       Vitamina tomada o día: " & tookVit.ToString() & VbCrLf )
#End if
    deferred.SportToday = False
    if fs = MorningExerciseMode.AlmostAlways Then
      deferred.SportToday = True
#If SlovakVersion Then
      Console.WriteLine("-----> " & "Ranné cvičenie dnes, potom")
#Else
      Console.WriteLine("-----> " & "Hoxe mañá exercicio, logo")  
#End if
    Else If If( isHeatingSeason(), dataForM.WashHeatWinter, dataForM.WashHeatMinutes )
      wash2do = True
      wash2time = DateTime.Now + New TimeSpan( 0, If( isHeatingSeason(), dataForM.WashHeatWinter, dataForM.WashHeatMinutes ), 0 )
      filled1bis = filled1bis + 1
#If SlovakVersion Then
      todo1(filled1bis) = "Umývanie " & string.Format("{0:HH':'mm}", wash2time)
#Else
      todo1(filled1bis) = "Lavarse " & string.Format("{0:HH':'mm}", wash2time) 
#End if
    Else
      filled1bis = filled1bis + 1
#If SlovakVersion Then
      todo1(filled1bis) = "Umývanie"
#Else
      todo1(filled1bis) = "Lavarse"
#End if
    End If
    If todo3 IsNot Nothing Then 
      filled1bis = filled1bis + 1
      todo1(filled1bis) = todo3
    End If
    If dataForM.LowCarbTeil.Length() > 0 Then
      If takeVit AndAlso fs = MorningExerciseMode.AlmostAlways Then
        filled1bis = filled1bis + 1
#If SlovakVersion Then
        todo1(filled1bis) = "Vezmite si včerajšie vitamíny"
#Else
        todo1(filled1bis) = "Toma as vitaminas de onte"
#End if
        justTookV = day146097-1
      End If
      Dim ur as String = dataForM.LowCarbTeil( day146097 mod dataForM.LowCarbTeil.Length() )
      filled1bis = filled1bis + 1
#If SlovakVersion Then
      todo1(filled1bis) = If( len(ur)<23, "V prípade potreby " & ur, ur )     
#Else
      todo1(filled1bis) = If( len(ur)<23, "Se é necesario " & ur, ur )     
#End if
    End If
    If fs <> MorningExerciseMode.AlmostAlways Then
      If DateTime.Today.DayOfWeek < dataForM.ZeitknapperAbTag OrElse DateTime.Today.DayOfWeek > dataForM.ZeitknapperBisTag Then
        If dataForM.OftenCleanClothing OrElse dataForM.OftenMatTraining OrElse day146097 mod 4 > 1 Then
          filled1bis = filled1bis + 2
          If day146097 mod 2 = 1 AndAlso ( (dataForM.SeldomCleanClothing AndAlso day146097 mod 8 > 3 ) OrElse dataForM.OftenCleanClothing ) Then
#If SlovakVersion Then
            todo1(filled1bis-1) = "Pripravte uvarené raňajky"
            todo1(filled1bis) = "Skontrolujte čistotu odevov"
#Else
            todo1(filled1bis-1) = "Cociña o almorzo"
            todo1(filled1bis) = "Verifique a limpeza da roupa"
#End if
          Else If dataForM.SeldomMatTraining OrElse dataForM.OftenMatTraining Then
#If SlovakVersion Then
            todo1(filled1bis-1) = "Mat cvičenia"
            todo1(filled1bis) = "Pripravte uvarené raňajky"
#Else
            todo1(filled1bis-1)  = "Exercicios de alfombra"
            todo1(filled1bis) = "Cociña o almorzo"
#End if
          Else
            filled1bis = filled1bis - 1
#If SlovakVersion Then
            todo1(filled1bis) = "Pripravte uvarené raňajky"
#Else
            todo1(filled1bis) = "Cociña o almorzo"
#End if
          End If
        Else
          filled1bis = filled1bis + 1
#If SlovakVersion Then
          todo1(filled1bis) = "Pripravte uvarené raňajky"
#Else
          todo1(filled1bis) = "Cociña o almorzo"
#End if
        End If
      End If
      If takeVit Then
        filled1bis = filled1bis + 1
#If SlovakVersion Then
        todo1(filled1bis) = "Vezmite si včerajšie vitamíny"
#Else
        todo1(filled1bis) = "Toma as vitaminas de onte"
#End if
        justTookV = day146097-1
      End If
      filled1bis = filled1bis + 1
#If SlovakVersion Then
      todo1(filled1bis) = If( dataForM.LowCarbTeil.Length() > 0 , "Hlavné raňajky" , "Raňajky" )
#Else
      todo1(filled1bis) = If( dataForM.LowCarbTeil.Length() > 0 , "Almorzo principal" , "Almorzo" )
#End if
    Else
      filled1bis = filled1bis + 1
#If SlovakVersion Then
      todo1(filled1bis) = If( (day146097 And 1) > 0 , "Mini kúsok koláča" , "Dajte buchtu do vrecka" )
#Else
      todo1(filled1bis) = If( (day146097 And 1) > 0 , "Mini anaco de bolo" , "Pon o bollo de leite no peto" )
#End if
      ' Wenn man vor dem Frühstück Sport macht, sollte man trotzdem minimal Kohlenhydrate essen, da der Körper sonst Eiweiss verbrennt
    End If
    Dim work1 as LocationOfDayWork
    if dataForM.WhereSundayToSaturday.Length() <> 7 Then errorExit( "WhereSundayToSaturday.Length()", Nothing )
    work1 = dataForM.WhereSundayToSaturday( day146097 mod 7 )
    if LocationOfDayWork.Question = work1 Then
      Console.WriteLine("      HomeOffice = 0
      HouseholdAndHomeOffice = 1
      ShoppingAndHousehold = 2
      Downtown = 3
      DayOff = 4
      EarlyShift = 5
      Study = 6
      ExternalTraining = 7" & VbCrLF)
      Dim line As String
      Do
#If SlovakVersion Then
        Console.Write("Vyberte prosím 0-7: ")
#Else
        Console.Write("Escolla 0-7: ")
#End if
        line = Console.ReadKey(true).KeyChar
      Loop Until "01234567".Contains(line) 
      work1 = cint(line)
      Console.WriteLine( line )
    End If
    Console.WriteLine(   "-----> " & work1.ToString() ) 
    Console.WriteLine(VbLf)
    Dim garbDayDiff As Integer? = Nothing
    If wForM.OneKindOfGarbDD>0 AndAlso wForM.OneKindOfGarbMM>0 Then
      Dim Ddd As DateTime
      If wForM.OneKindOfGarbYYYY = 0 Then 
        Ddd = New DateTime(DateTime.ToDay.Year    ,wForM.OneKindOfGarbMM,wForM.OneKindOfGarbDD)
      Else
        Ddd = New DateTime(wForM.OneKindOfGarbYYYY,wForM.OneKindOfGarbMM,wForM.OneKindOfGarbDD)
      End If
      garbDayDiff = DateDiff(DateInterval.Day, Ddd, DateTime.ToDay)
    End If
    If Not IsNothing(garbDayDiff) AndAlso ( garbDayDiff=0 OrElse (wForM.OneKOGEveryNWeeks>0 AndAlso garbDayDiff mod (7*wForM.OneKOGEveryNWeeks)=0 ) ) Then
      filled1bis = filled1bis + 1
#If SlovakVersion Then
      Console.WriteLine( "=====> " & "Odpadu" ) 
      todo1(filled1bis) = "Odpadu"
#Else
      Console.WriteLine( "=====> " & "Lixo" ) 
      todo1(filled1bis) = "Lixo"
#End if
    End If
    Dim PublicHoliday = False
    deferred.HolidayToday = False
    If isPublicHoliday(0) Then
#If SlovakVersion Then
      Console.WriteLine( "=====> " & "Verejne prazdniny ( Public Holiday )" ) 
#Else
      Console.WriteLine( "=====> " & "Día festivo  ( Public Holiday )" ) 
#End if
      Console.WriteLine(VbLf)
      PublicHoliday =  True
      deferred.HolidayToday = True
      filled1bis = 0
    End If
    deferred.HolidayTomorrow = False
    If isPublicHoliday(1) Then
      For iholiday = 1 To 3
#If SlovakVersion Then
        Console.WriteLine( "Verejne prazdniny Zajtra  ( Public Holiday tomorrow )" & " <=====") 
#Else
        Console.WriteLine( "Día festivo Mañá  ( Public Holiday tomorrow )" & " <====="  ) 
#End if
      Next
      Console.WriteLine(VbLf)
      deferred.HolidayTomorrow = True
    End If
    If DateTime.ToDay.DayOfYear = 114 AndAlso ((DateTime.ToDay.Year mod 4)>=1 OrElse (DateTime.ToDay.Year mod 20)=0 ) Then
      If (DateTime.ToDay.Year mod 20)=0 Then Console.WriteLine( "tomorrow " )
#If SlovakVersion Then
      Console.WriteLine( "=====> " & "( Arménsky deň spomienok )" ) 
#Else
      Console.WriteLine( "=====> " & "( Día de Remembrance Armenian )" ) 
#End if
    End If
    If wForM.URL_ofDay.Length() > 0 Then
      Dim url as String = wForM.URL_ofDay( day146097 mod wForM.URL_ofDay.Length() )
      Console.WriteLine( "-----> " & url ) 
      filled1bis = filled1bis + 1
#If SlovakVersion Then
      todo1(filled1bis) = If( len(url)<27, url, "Pozrite si vyššie uvedenú URL (U)")
#Else
      todo1(filled1bis) = If( len(url)<27, url, "Ver o URL anterior (U)")     
#End if
      todayurl = url
    End if  
    If DateTime.Now.Hour < 7 AndAlso DateTime.Today.DayOfWeek = DayOfWeek.Monday AndAlso Not PublicHoliday Then
      filled1bis = filled1bis + 1
#If SlovakVersion Then
      todo1(filled1bis) = "V prípade potreby zapnite umývačku riadu"
#Else
      todo1(filled1bis) = "Acenda o lavaplatos se é necesario"
#End if
    End If
    if fs = MorningExerciseMode.AlmostAlways Then
      filled1bis = filled1bis + 1
#If SlovakVersion Then
      todo1(filled1bis) = "Outdoorové športové oblečenie"     
#Else
      todo1(filled1bis) = "Poña roupa deportiva ao aire libre"     
#End if
    ElseIf (work1 = LocationOfDayWork.ShoppingAndHousehold OrElse work1 = LocationOfDayWork.Downtown OrElse 
           work1=LocationOfDayWork.EarlyShift OrElse work1=LocationOfDayWork.ExternalTraining) AndAlso 
            Not PublicHoliday Then
      filled1bis = filled1bis + 1
#If SlovakVersion Then
      todo1(filled1bis) = "Outdoorové oblečenie"
#Else
      todo1(filled1bis) = "Desgaste roupa para exteriores"
#End if
    End If
    Dim gt As Integer = dataForM.WaterPlantsAfterDaysQ2Q3
    If DateTime.Now.Month <= 3 OrElse DateTime.Now.Month > 9 Then gt = dataForM.WaterPlantsAfterDaysQ1Q4
    If isHeatingSeason() Then 
      gt = dataForM.WaterInHeatingSeason
    End If
    If deferred.SeedingDay    <= DateTime.ToDay.DayOfYear + 365*((DateTime.ToDay.Year-1) mod 4 ) AndAlso 
       deferred.SeedingDay+14 >= DateTime.ToDay.DayOfYear + 365*((DateTime.ToDay.Year-1) mod 4 ) AndAlso 
       dataForM.Water4SeedingInDay4to14 >= 1 Then
      gt = dataForM.Water4SeedingInDay4to14
    End If
    If gt <= 0 Then gt = 99999999
    Dim giessen As Boolean = (((day146097+17) mod gt)=0) ' Alle gt Tage, wird prinzipiell gegossen aber es gibt Ausnahmen
    Dim day_mod As Integer = day146097+17
    If dataForM.ZeitknapperBisTag >= dataForM.ZeitknapperAbTag Then ' Am Rande des Zeitraums
      If (DateTime.Today.DayOfWeek+1) mod 7 = dataForM.ZeitknapperAbTag Then ' giessen auf den Tag ausserhalb schieben
        day_mod = day_mod + 1
      ElseIf DateTime.Today.DayOfWeek = (dataForM.ZeitknapperBisTag+1) mod 7 Then
        day_mod = day_mod - 1
      End If
      If DateTime.Today.DayOfWeek = dataForM.ZeitknapperBisTag Then
         giessen = False ' Dafür an den Tagen drum rum zwei Varianten erhöhte Wahrscheinlichkeit:
      Else If (DateTime.Today.DayOfWeek+1 mod 7) = dataForM.ZeitknapperAbTag OrElse DateTime.Today.DayOfWeek = (dataForM.ZeitknapperBisTag+1)mod 7 Then
         giessen = giessen OrElse ((day_mod mod gt)=0)
      End If
      If deferred.WaterPlants1 Then giessen = True
    End If
    If fs <> MorningExerciseMode.AlmostAlways AndAlso dataForM.Brush Then
      filled1bis = filled1bis + 1
#If SlovakVersion Then
      todo1(filled1bis) = "Čistite si zuby"
#Else
      todo1(filled1bis) = "Cepillo de dentes"
#End if
    End If
    deferred.WaterLater = False
    If giessen Then
      filled1bis = filled1bis + 1
      If DateTime.Today.DayOfWeek >= dataForM.ZeitknapperAbTag AndAlso DateTime.Today.DayOfWeek <= dataForM.ZeitknapperBisTag Then
#If SlovakVersion Then
        todo1(filled1bis) = "Zalievanie rastlín -> Poobede"
#Else
        todo1(filled1bis) = "Encha o Regadeira" ' Fill the Watering can, to remind to water in the afternoon
#End if
        deferred.WaterLater = True
      Else
#If SlovakVersion Then
        todo1(filled1bis) = "Zalievanie rastlín"
#Else
        todo1(filled1bis) = "Plantas de rego"
#End if
        deferred.WaterPlants1 = True
      End If
#If SlovakVersion Then
      Console.WriteLine("-> " & "Zalievanie rastlín (dnešok)")
#Else
      Console.WriteLine("-> " & "Plantas de rego (hoxe)")  
#End if
    End If
    If dataForM.LowCarbTeil.Length() = 0 AndAlso fs = MorningExerciseMode.AlmostAlways  AndAlso  takeVit Then
        filled1bis = filled1bis + 1
#If SlovakVersion Then
        todo1(filled1bis) = "Vezmite si včerajšie vitamíny"
#Else
        todo1(filled1bis) = "Toma as vitaminas de onte"
#End if
        justTookV = day146097-1
    End If
    If dataForM.Freezer AndAlso ((day146097+57) mod 5)=0 Then
        filled1bis = filled1bis + 1
#If SlovakVersion Then
        todo1(filled1bis) = "Rozmrazte bobule"
#Else
        todo1(filled1bis) = "Descongela as bagas"
#End if
        justTookV = day146097-1
    ElseIf dataForM.Freezer AndAlso (((day146097+57)*4) mod 13)<4 Then ' on average every 4 days given 1/5 less
        filled1bis = filled1bis + 1
        If DateTime.ToDay.Month >= 6 AndAlso DateTime.ToDay.Month <= 11 Then
#If SlovakVersion Then
          todo1(filled1bis) = "Rozmrazte 110 g kelu alebo krémového špenátu"
#Else
          todo1(filled1bis) = "Descongela 110g a espinaca ou a col rizada"
#End if
        Else
#If SlovakVersion Then
          todo1(filled1bis) = "Rozmrazte 150 g kelu alebo krémového špenátu"
#Else
          todo1(filled1bis) = "Descongela 150g a espinaca ou a col rizada"
#End if
        End If
        justTookV = day146097-1
    End If
  End Sub

 
  Function PhysicalActivityDays( day146097 As Integer, regelstruct As DataForMorningP ) As MorningExerciseMode
    Dim laenge = regelstruct.MorningExerciseModeArray.Length()
    if 0=laenge Then Return MorningExerciseMode.AlmostAlways
    Dim fs as MorningExerciseMode
    fs = regelstruct.MorningExerciseModeArray( day146097 mod laenge )
    ' MorningExerciseMode auf NoMorningExercise, WeatherQuestion und AlmostAlways reduzieren
    If MorningExerciseMode.SummerHalfYear = fs AndAlso DateTime.Today.Month >= 5 AndAlso DateTime.Today.Month <= 10 Then
      fs = MorningExerciseMode.AlmostAlways
    Elseif MorningExerciseMode.SummerHalfYear = fs then
      fs = MorningExerciseMode.NoMorningExercise
    Elseif MorningExerciseMode.WeatherFromApril = fs AndAlso DateTime.Today.Month >= 4 then
      fs = MorningExerciseMode.WeatherQuestion
    Elseif MorningExerciseMode.WeatherFromApril = fs then
      fs = MorningExerciseMode.NoMorningExercise
    End If
    Return fs
  End Function

  Dim left() as String
  Dim r() as String

 
  Function janein1(text As String)
    Dim line As String
    Do
#If SlovakVersion Then
      Console.Write("{0} ? (á=y/ž=n) ", text)
#Else
      Console.Write("{0} ? (s=y/n) ", text)
#End if
     line = Console.ReadKey(true).KeyChar
     If line.ToUpper() = "U" OrElse line.ToUpper() = "X" then 
       Console.write( VbCrLf &  "-URL-" & VbCrLf )
       ShowURL
     End if
     If line.ToUpper() = "X" then 
#If SlovakVersion Then
      Console.WriteLine("X also means premature Exit")    
      Console.WriteLine("Program ukončíte stlačením klávesu.")
#Else
      Console.WriteLine("X also means premature Exit")    
      Console.WriteLine( "Prema unha tecla para rematar o programa." )
#End if
      Console.ReadKey()
      Environment.Exit(-1)
    End if
    Loop Until "aáAÁzžZŽjnJNsSyY".Contains(line) ' slovak umlauts for á Á are &#225; &#193; 
    If line = "ž" OrElse line="Ž" Then Line = "z" ' ToUpper does not work with Character number &#382; or &#381;
    If line.ToUpper() = "A"  OrElse line.ToUpper() = "Á"  OrElse line.ToUpper() = "J"  OrElse line.ToUpper() = "S"  OrElse line.ToUpper() = "Y" then 
#If SlovakVersion Then
      Console.WriteLine(" áno ")
#Else
      Console.WriteLine(" Si ")
#End if
      Return True
    Else 
#If SlovakVersion Then
      Console.WriteLine(" žiadny ")
#Else
      Console.WriteLine(" Non ")
#End if
      Return False
    End If
  End Function



  Private Sub ShowURL
    Dim day146097 As Integer = DateDiff(DateInterval.Day, GlobalConstants.base_sunday, Date.Now )
    day146097 = day146097 mod 146097
#If SlovakVersion Then
    If wForM.URL_ofDay Is Nothing OrElse wForM.URL_ofDay.Length() = 0 Then 
      Console.WriteLine("Varovanie: URL_ofDay nie je k dispozícii v daten_fuer_morgen.xml")
#Else
    If wForM.URL_ofDay Is Nothing OrElse wForM.URL_ofDay.Length() = 0 Then 
      Console.WriteLine("Aviso: Non está presente URL_ofDay en data1_for_morning_galician.xml?")
#End if
    Else 
      Dim url as String = wForM.URL_ofDay( day146097 mod wForM.URL_ofDay.Length() )
      Try
        If Not IsNothing( url ) Then
          dim psi as new ProcessStartInfo( url )
          psi.UseShellExecute = true
          System.Diagnostics.Process.Start( psi )
        End if  
      Catch ee As Exception
        errorExit( url  , ee )
      End Try
    End If
  End Sub



  Private Sub dozweispaltigchecklist( ByVal left() As String, ByVal r() As String, url As String )
  ' fills in timestep2, timestep3 because this is website reading. website reading time and can vary.
#If SlovakVersion Then
    Console.WriteLine( VbCrLf & "Odpovedzte, ktorá strana bola spracovaná (L ou R, Ľ ou P)" ) ' Ľ = LETTER L WITH CARON #317, ľ = #318
#Else
    Console.WriteLine( VbCrLf & "Responda de que lado se procesou (L ou R, E ou D)" )
#End if
    If Not IsNothing(url) Then
#If SlovakVersion Then
      Console.WriteLine( "Na vyvolanie URL je možné zadať U.")
#Else
      Console.WriteLine( "Entre U pode introducirse para chamar á URL.")
#End if
    End If
    Dim lindex as Integer = 1
    Dim rindex as Integer = 1
    Dim line As String
#If SlovakVersion Then
    Console.WriteLine("Spracovať je {0} + {1} položiek."    & VbCrLf  & VbCrLf & VbCrLf & VbCrLf, left.Length-1, r.Length-1)
#Else
    Console.WriteLine("Hai {0} + {1} elementos a procesar." & VbCrLf  & VbCrLf & VbCrLf & VbCrLf, left.Length-1, r.Length-1)
#End if
    Dim spaceline = new String( " "c, 78)
    Dim oneStepAdded = False
    Do While lindex < left.Length OrElse rindex < r.Length
     Console.Write(vbcr &  spaceline )
     Do
      If lindex < left.length AndAlso rindex < r.length AndAlso len(left(lindex))+len(r(rindex))>73 then
        Console.Write(vbcr &  "{0}, {1} ?"& upr, left(lindex).SubString(0,Math.Max(3,73-len(r(rindex)))) ,r(rindex))
      Elseif lindex < left.length AndAlso rindex < r.length AndAlso len(left(lindex))+len(r(rindex))>63 then
        Console.Write(vbcr &  "{0}, {1} ?"& upr, left(lindex),r(rindex))
#If SlovakVersion Then
      Elseif lindex < left.length AndAlso rindex < r.length AndAlso len(left(lindex))+len(r(rindex))>44 then
        Console.Write(vbcr &  "{0}. {1} ?  (L/P)"& upr, left(lindex),r(rindex))
      ElseIf lindex < left.length AndAlso rindex < r.length then
        Console.Write(vbcr &  "{0}.: {1}, {2} ? (Ľavá,Pravá)" & upr, nsteps+1, left(lindex),r(rindex))
      Elseif lindex < left.length  AndAlso len(left(lindex))>39  then
        Console.Write(vbcr &  "{0}, - ? (P)"& upr, left(lindex))
      Elseif lindex < left.length then
        Console.Write(vbcr &  "{0},  a potom stlačte Ľ = ľ = L ."& upr, left(lindex))
      Else
        Console.Write(vbcr &  "Po {0} stlačte R alebo P  ?"& upr,  r(rindex))
#Else
      Elseif lindex < left.length AndAlso rindex < r.length AndAlso len(left(lindex))+len(r(rindex))>41 then
        Console.Write(vbcr &  "{0},    {1} ?  (E/D)"& upr, left(lindex),r(rindex))
      ElseIf lindex < left.length AndAlso rindex < r.length then
        Console.Write(vbcr &  "{0}.: {1}, {2} ? (Esquerda,Dereita)"& upr, nsteps+1, left(lindex),r(rindex))
      Elseif lindex < left.length  AndAlso len(left(lindex))>39  then
        Console.Write(vbcr &  "{0}, - ? (E)"& upr, left(lindex))
      Elseif lindex < left.length then
        Console.Write(vbcr &  "{0},  logo prema E = L ?"& upr, left(lindex))
      Else
        Console.Write(vbcr &  "Pulse R ou D despois do {0} ?"& upr,  r(rindex))
#End if
      End if
      line = Console.ReadKey(true).KeyChar 
      If lindex = 1 AndAlso Rindex=1 AndAlso  line.ToUpper() = "X" Then 
       Console.write( VbCrLf &  "-URL-" & VbCrLf )
       ShowURL
#If SlovakVersion Then
       Console.WriteLine("X also means premature Exit")    
       Console.WriteLine("Program ukončíte stlačením klávesu.")
#Else
       Console.WriteLine("X also means premature Exit")    
       Console.WriteLine( "Prema unha tecla para rematar o programa." )
#End if
       Console.ReadKey()
       Environment.Exit(-1)
      End if
#If SlovakVersion Then
     Loop Until "lLľĽpPrRuvstUVST".Contains(line)
#Else
     Loop Until "lLeEdDrRuvstUVST".Contains(line)
#End if
     upr = " "
     If line = "ľ" OrElse line="Ľ" Then Line = "L" ' ToUpper does not work with Character number 318, 317
     If line.ToUpper() = "R" OrElse line.ToUpper() = "P" OrElse line.ToUpper() = "D" then 
      If rindex < r.Length Then nsteps = nsteps + 1
      rindex = rindex  + 1
     Elseif line.ToUpper() = "S" then ' For skipping a Step: Pressing S neutralises Time Measurement of a step.
       If nsteps > 2 OrElse (nsteps > 0 AndAlso lindex < left.length-2) Then 
         nsteps = nsteps - 1
       End If
     Elseif line.ToUpper() = "T" then ' One additional step which was not planned.
       If oneStepAdded = False Then 
         nsteps = nsteps + 1
         oneStepAdded = True
       End If
     Elseif line.ToUpper() = "U" then 
       If timestamp2 = Nothing Then 
         timestamp2 = DateTime.Now
         nsteps = nsteps - 1
       End If
       If  upr=" " Then
         Console.write( vbcr &  "...                                                                         ")
         ' ... will be overridden after Process.Start
         upr = "…" ' Unicode Character : horizontal ellipsis, appears after Process.Start
       End if
       ShowURL
     Elseif line.ToUpper() = "V" then 
       Console.write( vbcr &  "SaveViewLater ...  ")
       SaveViewLater( url )
     Else 
      If lindex < left.length Then nsteps = nsteps + 1
      lindex  = lindex  + 1
     End If
     If line.ToUpper() <> "U" And timestamp2 <> Nothing And timestamp3 = Nothing Then timestamp3 = DateTime.Now
    Loop
#If SlovakVersion Then
    Console.WriteLine( VbCrLf & VbCrLf & "Pripravený." & VbCrLf )
#Else
    Console.WriteLine( VbCrLf & VbCrLf & "Feito." & VbCrLf )
#End if
  End Sub
  
  Private upr as String = " " ' Space or one threedot character as indication to wait


  Function isPublicHoliday(ByVal tomorrow As Integer)
    If dataForM.PublicHolidaysMDD.Length()=0 Then Return False
    Dim td = DateTime.Today
    If tomorrow Then td = td.AddDays(1)
    Dim mon = td.Month
    Dim day = td.Day
    For index as Integer = 0 to dataForM.PublicHolidaysMDD.Length()-1
      If dataForM.PublicHolidaysMDD(index)\100 = mon AndAlso day = dataForM.PublicHolidaysMDD(index) mod 100  Then Return td.DayOfWeek<>0
    Next
    If td.DayOfWeek = 5 AndAlso CalcGoodFriday( td.Year ) = td Then Return dataForM.PublicHolidayGoodFriday 
    If td.DayOfWeek = 1 Then
      If CalcGoodFriday( td.Year ).AddDays(3) = td Then Return dataForM.PublicHolidayEasterMonday
      If CalcGoodFriday( td.Year ).AddDays(52) = td Then Return dataForM.PublicHolidayWhitMonday
    End If    
    If td.DayOfWeek = 4 Then
      If CalcGoodFriday( td.Year ).AddDays(41) = td Then Return dataForM.PublicHolidayAscension
      If CalcGoodFriday( td.Year ).AddDays(62) = td Then Return dataForM.PublicHolidayCorpusChristi
    End If    
    Return False
  End Function
  
  
  Function CalcGoodFriday(ByVal year As Integer) As DateTime
        Dim a = year Mod 19
        Dim b = year \ 100
        Dim c = year Mod 100
        Dim d = b \ 4
        Dim e = b Mod 4
        Dim f = (b + 8) \ 25
        Dim g = (b - f + 1) \ 3
        Dim h = (19 * a + b - d - g + 15) Mod 30
        Dim i = c \ 4
        Dim k = c Mod 4
        Dim l = (32 + 2 * e + 2 * i - h - k) Mod 7
        Dim m = (a + 11 * h + 22 * l) \ 451
        Dim gregor = h + l - 7 * m + 112
        Dim month = gregor \ 31
        Dim day = (gregor Mod 31) + 1
        Return New DateTime(year, month, day)
  End Function


  Sub errorExit( text1 as String, text2 as Exception )
      Console.WriteLine(text1)    
      if text2 IsNot Nothing Then Console.WriteLine(text2)
#If SlovakVersion Then
      Console.WriteLine("Program ukončíte stlačením klávesu.")
#Else
      Console.WriteLine( "Prema unha tecla para rematar o programa." )
#End if
      Console.ReadKey()
      Environment.Exit(-1)
  End Sub
End Module


' Unfortionately I can not provide documentation of levante-se.vb
' except if someone pays me to document this program (whats not its purpose)




Public Module Interf
  Public Enum LocationOfDayWork
    HomeOffice = 0
    HouseholdAndHomeOffice = 1
    ShoppingAndHousehold = 2
    Downtown = 3
    DayOff = 4
    EarlyShift = 5
    Study = 6
    ExternalTraining = 7
    Question = 8
  End Enum
  
  
  Public Enum MorningExerciseMode
    NoMorningExercise
    SummerHalfYear
    WeatherQuestion
    WeatherFromApril
    AlmostAlways
  End Enum

  Public Structure DataForMorningP
    Public WhereSundayToSaturday() As LocationOfDayWork REM Anzahl 7 kann ich hier nicht festlegen.
    Public LowCarbTeil()  As String
    Public ZeitknapperAbTag As Integer
    Public ZeitknapperBisTag As Integer
    Public DayOfLastWalkOrJogging As Integer
    Public MorningExerciseModeArray() As MorningExerciseMode
    Public SeldomCleanClothing As Boolean
    Public SeldomMatTraining As Boolean
    Public OftenCleanClothing As Boolean
    Public OftenMatTraining As Boolean
    Public WaterPlantsAfterDaysQ1Q4 As Integer
    Public WaterPlantsAfterDaysQ2Q3 As Integer
    Public WaterInHeatingSeason As Integer
    Public Water4SeedingInDay4to14 As Integer
    Public WaterWhenTimeTight As Boolean
    Public WinterHeatingAbMMDD As Integer
    Public WinterHeatingBisMDD As Integer
    Public WashHeatMinutes As Integer
    Public WashHeatWinter As Integer
    Public Brush As Boolean
    Public Freezer As Boolean
    Public TkWeight As Boolean
    Public PublicHolidaysMDD() As Integer
    Public PublicHolidayGoodFriday As Boolean
    Public PublicHolidayEasterMonday As Boolean
    Public PublicHolidayAscension As Boolean
    Public PublicHolidayWhitMonday As Boolean
    Public PublicHolidayCorpusChristi As Boolean
  End Structure

  Public Structure WorteForMorningP
    Public VersionOfStruct As Decimal
    Public TodoPart2() As String
    Public SaveAStringPrompt As String
    Public OneKindOfGarbDD As Integer
    Public OneKindOfGarbMM As Integer
    Public OneKindOfGarbYYYY As Integer
    Public OneKOGEveryNWeeks As Integer
    Public URL_ofDay() As String
  End Structure
  
  Public Structure VTupel
    Public Property  D As Decimal ' JsonSerializer works best with Properties
    Public Property  V As Integer
  End Structure
  
  Public Structure StreetStatist
    Public Property  C As Decimal
    Public Property  MuchTraffic As List(Of VTupel)
  End Structure

  Public Structure DeferredFor
    Public WaterLater As Boolean
    Public WaterPlants1 As Boolean
    Public SportToday As Boolean
    Public SeedingDay As Single ' DayOf 4 Years = Day of Quadrennial
    Public LastSport As Integer ' currently not used
    Public HolidayToday As Boolean
    Public HolidayTomorrow As Boolean
 End Structure
End Module



Public Module GlobalConstants
    Public ReadOnly base_sunday As Date = New Date(2018, 9, 2) ' Muss ein Sonntag sein =Tag 0. Die Zahl der Tage ab dem 31.12.1899 ist durch 63 teilbar.
End Module
