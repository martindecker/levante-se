Imports System
Imports System.Xml.Serialization
Imports System.IO
Imports System.Text

' User language = Galician or Slowak, Enums and the Winter Keyword = English
#Const SlovakVersion = False

Module Program
  Public todo1(20) As String
  Public filled1bis As Integer = 0
  Public todo3 As String = Nothing
  Dim todayurl = Nothing
  Dim timestamp1,timestamp2,timestamp3,timestamp4 As DateTime ' used time = t2-t1 + t4-t3
  Dim nsteps as Integer = 0 ' how many steps in that time ?

  Sub Main(args As String())
      LoadAData
      If DateTime.Today.DayOfWeek = DayOfWeek.Sunday Then
#If SlovakVersion Then
          Dim stoerungen2 = janein1("Bola cesta zle využitá")
          If  stoerungen2 Then
              todo3 = "Zistite príčinu rušnej ulice!"
          Else
              Console.WriteLine("ok")
          End If
#Else
          Dim stoerungen2 = janein1("¿Tráfico pesado na estrada")
          If  stoerungen2 Then
              todo3 = "Descubra a causa da estrada transitada"
          Else
              Console.WriteLine("nada")
          End If
#End if
      End If
      Console.WriteLine(VbLf)
      PlanningQuestions
      Array.Resize( todo1,filled1bis+1 )
      left = todo1
      r = wForM.TodoPart2
      If Not isHeatingSeason() Then 
          r = Array.FindAll( wForM.TodoPart2, Function(p As String ) IsNothing(p) OrElse Not p.StartsWith("Winter:") )
      End If
      timestamp1 = DateTime.Now
      dozweispaltigchecklist( todayurl ) 
      timestamp4 = DateTime.Now
      Dim interval As TimeSpan
      If timestamp2 <> Nothing AndAlso timestamp3 <> Nothing Then
        interval = (timestamp2 - timestamp1)+(timestamp4 - timestamp3)
      Else
        interval = timestamp4 - timestamp1
      End If
#If SlovakVersion Then
      Console.Write("Potrebovali ste {0} minút na {1} krokov ", interval.Hours * 60 + interval.Minutes, nsteps )
      If nsteps > 0 Then Console.WriteLine("= {0:N1} minút na krok. ", interval.TotalMinutes/nsteps )
      Console.WriteLine("Zadajte niečo na ukončenie programu.")
#Else
      Console.Write("Tardaron {0} minutos en {1} pasos ", interval.Hours * 60 + interval.Minutes, nsteps )
      If nsteps > 0 Then Console.WriteLine("= {0:N1} minutos por paso. ", interval.TotalMinutes/nsteps )
      Console.WriteLine("Prema unha tecla para rematar o programa.")
#End If
      Console.ReadKey()
  End Sub
    
  Dim dataForM as DataForMorningP
  Dim wForM    as WorteForMorningP


  Private Sub LoadAData
#If SlovakVersion Then
    Dim fn = "jsonxmlm/data1_for_morning_slovak.xml"
#Else
    Dim fn = "jsonxmlm/data1_for_morning_galician.xml"
#End If
    Try
      For Teste = 1 To 5
        If not File.Exists(fn) Then fn = "../" & fn
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
    if fs = MorningExerciseMode.AlmostAlways Then
#If SlovakVersion Then
      Console.WriteLine("-----> " & "Ranné cvičenie dnes, potom")
#Else
      Console.WriteLine("-----> " & "Hoxe mañá exercicio, logo")  
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
      filled1bis = filled1bis + 1
#If SlovakVersion Then
      todo1(filled1bis) = If( dataForM.LowCarbTeil.Length() > 0 , "Hlavné raňajky" , "Raňajky" )
#Else
      todo1(filled1bis) = If( dataForM.LowCarbTeil.Length() > 0 , "Almorzo principal" , "Almorzo" )
#End if
    Else
      filled1bis = filled1bis + 1
#If SlovakVersion Then
      todo1(filled1bis) = If( day146097 And 1 > 0 , "Mini kúsok koláča" , "Dajte buchtu do vrecka" )
#Else
      todo1(filled1bis) = If( day146097 And 1 > 0 , "Mini anaco de bolo" , "Pon o bollo de leite no peto" )
#End if
      ' Wenn man vor dem Frühstück Sport macht, sollte man trotzdem minimal Kohlenhydrate essen, da der Körper sonst Eiweiss verbrennt
    End If
    Dim work1 as LocationOfDayWork
    if dataForM.WhereSundayToSaturday.Length() <> 7 Then errorExit( "WhereSundayToSaturday.Length()", Nothing )
    work1 = dataForM.WhereSundayToSaturday( day146097 mod 7 )
    Console.WriteLine(   "-----> " & work1.ToString() ) 
    Console.WriteLine(VbLf)
    Dim PublicHoliday = False
    If isPublicHoliday() Then
      Console.WriteLine( "=====> " & "Public Holiday" ) 
      Console.WriteLine(VbLf)
      PublicHoliday =  True
      filled1bis = 0
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
    ElseIf 4 <= DateTime.Today.Day AndAlso DateTime.Today.Day <= 14 Then
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
    End If
    If giessen Then
      filled1bis = filled1bis + 1
      If DateTime.Today.DayOfWeek >= dataForM.ZeitknapperAbTag AndAlso DateTime.Today.DayOfWeek <= dataForM.ZeitknapperBisTag Then
#If SlovakVersion Then
        todo1(filled1bis) = "Zalievanie rastlín -> Poobede"
#Else
        todo1(filled1bis) = "Encha o rego" ' Fill the Watering can, to remind to water in the afternoon
#End if
      Else
#If SlovakVersion Then
        todo1(filled1bis) = "Zalievanie rastlín"
#Else
        todo1(filled1bis) = "Plantas de rego"
#End if
      End If
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
  
  
  Private Sub dozweispaltigchecklist( url As String )
  ' Uses Arrays left() As String,r() As String
  ' fills in timestep2, timestep3 because this is website reading time and can vary.
#If SlovakVersion Then
    Console.WriteLine( VbCrLf & " Odpovedzte, ktorá strana bola spracovaná (L ou R, Ľ ou P)" & vbCrlf) ' Ľ = LETTER L WITH CARON #317, ľ = #318
#Else
    Console.WriteLine( VbCrLf & " Responda de que lado se procesou (L ou R, E ou D)" & vbCrlf)
#End if
    If Not IsNothing(url) Then
#If SlovakVersion Then
      Console.WriteLine( "Na vyvolanie URL je možné zadať U."& vbCrlf)
#Else
      Console.WriteLine( "Entre U pode introducirse para chamar á URL."& vbCrlf)
#End if
    End If
    Dim lindex as Integer = 1
    Dim rindex as Integer = 1
    Dim line As String
#If SlovakVersion Then
    Console.WriteLine("Spracovať je {0} + {1} položiek." & VbCrLf & VbCrLf & VbCrLf  & VbCrLf & VbCrLf & VbCrLf, left.Length-1, r.Length-1)
#Else
    Console.WriteLine("Hai {0} + {1} elementos a procesar." & VbCrLf & VbCrLf & VbCrLf  & VbCrLf & VbCrLf & VbCrLf, left.Length-1, r.Length-1)
#End if
    Dim spaceline = new String( " "c, 78)
    Do While lindex < left.Length OrElse rindex < r.Length
     Console.Write(vbcr &  spaceline )
     Do
      If lindex < left.length AndAlso rindex < r.length AndAlso len(left(lindex))+len(r(rindex))>73 then
        Console.Write(vbcr &  "{0}, {1} ?"& upr, left(lindex).SubString(0,Math.Max(3,73-len(r(rindex)))) ,r(rindex))
      Elseif lindex < left.length AndAlso rindex < r.length AndAlso len(left(lindex))+len(r(rindex))>63 then
        Console.Write(vbcr &  "{0}, {1} ?"& upr, left(lindex),r(rindex))
      Elseif lindex < left.length AndAlso rindex < r.length AndAlso len(left(lindex))+len(r(rindex))>44 then
#If SlovakVersion Then
        Console.Write(vbcr &  "{0},    {1} ?  (L/P)"& upr, left(lindex),r(rindex))
      ElseIf lindex < left.length AndAlso rindex < r.length then
        Console.Write(vbcr &  "{0}, {1} ? (Ľavá,Pravá)"& upr, left(lindex),r(rindex))
      Elseif lindex < left.length  AndAlso len(left(lindex))>39  then
        Console.Write(vbcr &  "{0}, - ? (P)"& upr, left(lindex))
      Elseif lindex < left.length then
        Console.Write(vbcr &  "{0},  a potom stlačte Ľ = ľ = L ."& upr, left(lindex))
      Else
        Console.Write(vbcr &  "Po {0} stlačte R alebo P  ?"& upr,  r(rindex))
#Else
        Console.Write(vbcr &  "{0},    {1} ?  (E/D)"& upr, left(lindex),r(rindex))
      ElseIf lindex < left.length AndAlso rindex < r.length then
        Console.Write(vbcr &  "{0}, {1} ? (Esquerda,Dereita)"& upr, left(lindex),r(rindex))
      Elseif lindex < left.length  AndAlso len(left(lindex))>39  then
        Console.Write(vbcr &  "{0}, - ? (E)"& upr, left(lindex))
      Elseif lindex < left.length then
        Console.Write(vbcr &  "{0},  logo prema E = L ?"& upr, left(lindex))
      Else
        Console.Write(vbcr &  "Pulse R ou D despois do {0} ?"& upr,  r(rindex))
#End if
      End if
      line = Console.ReadKey(true).KeyChar 
#If SlovakVersion Then
     Loop Until "lLľĽpPrRuU".Contains(line)
#Else
     Loop Until "lLeEdDrRuU".Contains(line)
#End if
     upr = " "
     If line = "ľ" OrElse line="Ľ" Then Line = "L" ' ToUpper does not work with Character number 318, 317
     If line.ToUpper() = "R" OrElse line.ToUpper() = "P" OrElse line.ToUpper() = "D" then 
      If rindex < r.Length Then nsteps = nsteps + 1
      rindex = rindex  + 1
     Elseif line.ToUpper() = "U" then 
       If timestamp2 = Nothing Then 
         timestamp2 = DateTime.Now
         nsteps = nsteps - 1
       End If
       Try
         If Not IsNothing(url) Then
           dim psi as new ProcessStartInfo(url)
           psi.UseShellExecute = true
           If  upr=" " Then
             Console.write( vbcr &  "...                                                                         ")
             ' ... will be overridden after Process.Start
             upr = "…" ' Unicode Character : horizontal ellipsis, appears after Process.Start
           End if
           System.Diagnostics.Process.Start( psi )
         End If
       Catch ee As Exception
         errorExit( url  , ee )
       End Try
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


  Function isPublicHoliday()
    If dataForM.PublicHolidaysMDD.Length()=0 Then Return False
    Dim td = DateTime.Today
    Dim mon = td.Month
    Dim day = td.Day
    For index as Integer = 0 to dataForM.PublicHolidaysMDD.Length()-1
      If dataForM.PublicHolidaysMDD(index)\100 = mon AndAlso day = dataForM.PublicHolidaysMDD(index) mod 100  Then Return DateTime.Today.DayOfWeek<>0
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



Public Module Interf
  Public Enum LocationOfDayWork
    HomeOffice = 0            '
    HouseholdAndHomeOffice = 1'
    ShoppingAndHousehold = 2  '
    Downtown = 3              '
    DayOff = 4                  '
    EarlyShift = 5          '
    Study = 6                '
    ExternalTraining = 7  '
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
    Public PublicHolidaysMDD() As Integer
    Public PublicHolidayGoodFriday As Boolean
    Public PublicHolidayEasterMonday As Boolean
    Public PublicHolidayAscension As Boolean
    Public PublicHolidayWhitMonday As Boolean
    Public PublicHolidayCorpusChristi As Boolean
  End Structure

  Public Structure WorteForMorningP
    Public VersionOfStruct As Decimal
    Public TodoPart2() as String
    Public URL_ofDay() As String
  End Structure
End Module

Public Module GlobalConstants
    Public ReadOnly base_sunday As Date = New Date(2018, 9, 2) ' Muss ein Sonntag sein =Tag 0. Die Zahl der Tage ab dem 31.12.1899 ist durch 63 teilbar.
End Module
