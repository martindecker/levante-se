Imports System
Imports System.Xml.Serialization
Imports System.IO
Imports System.Text

Module Program
    Public todo2(20) As String
    Public filled2bis As Integer = 0
    Public todo3 As String = Nothing

    Sub Main(args As String())
        LoadAData
        If DateTime.Today.DayOfWeek = DayOfWeek.Saturday Then
          Dim stoerungen2 = janein1("Straße arg befahren")
          If  stoerungen2 Then
              todo3 = "Ursache fuer stark befahrene Straße rausfinden"
          Else
              Console.WriteLine("nix")
          End If
        End If
        Console.WriteLine(VbLf)
        Planungsfragen
        Array.Resize( todo2,filled2bis+1 )
        left = todo2
        r = wForM.OftR
		If Not istHeizperiode() Then 
		  r = Array.FindAll( wForM.OftR, Function(p As String ) IsNothing(p) OrElse Not p.StartsWith("Winter:") )
		End If
        dozweispaltigchecklist
        ' dozweispaltigchecklist( todo2, wForM.OftR )
        Console.WriteLine("Please enter sth to end the program.")
        Console.ReadKey()
    End Sub
    
  Dim dataForM as DataForMorningP
  Dim wForM    as WorteForMorningP


  Private Sub LoadAData
    Dim fn = "jsonxmlm/daten_fuer_morgen.xml"
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
      fn = "jsonxmlm/worte_fuer_morgen.xml"
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
      Console.WriteLine( "/jsonxmlm/*.xml" & " geladen" )
      If dataForM.WhereSundayToSaturday Is Nothing Then Console.WriteLine("Warnung:  ist in daten_fuer_morgen.xml nicht vorhanden") 
      If dataForM.URL_ofDay Is Nothing Then Console.WriteLine("Warnung:  ist in daten_fuer_morgen.xml nicht vorhanden")
      If dataForM.FruehsportModeArray Is Nothing Then Console.WriteLine("Warnung:  ist in daten_fuer_morgen.xml nicht vorhanden")
    Catch eee As Exception  
      errorAbbruch(fn & "   Error:", eee)
    End Try 
  End Sub

  
  Function istHeizperiode()
    Dim mon = DateTime.Now.Month
    Dim day = DateTime.Now.Day
    If dataForM.WinterHeizAbMMDD < dataForM.WinterHeizBisMDD Then
	  If dataForM.WinterHeizAbMMDD/100 < mon AndAlso mon < dataForM.WinterHeizBisMDD/100 Then Return true
	  If dataForM.WinterHeizAbMMDD/100 = mon Then Return day > dataForM.WinterHeizAbMMDD mod 100
	  If dataForM.WinterHeizBisMDD/100  = mon Then Return day < dataForM.WinterHeizBisMDD mod 100
	  Return False
	Else
	  If dataForM.WinterHeizAbMMDD/100 > mon OrElse mon < dataForM.WinterHeizBisMDD/100 Then Return true
	  If dataForM.WinterHeizAbMMDD/100 = mon Then Return day > dataForM.WinterHeizAbMMDD mod 100
	  If dataForM.WinterHeizBisMDD/100  = mon Then Return day < dataForM.WinterHeizBisMDD mod 100
	  Return False
    End If
  End Function
  
  
  Sub Planungsfragen
    Dim day146097 As Integer = DateDiff(DateInterval.Day, GlobalConstants.base_sunday , Date.Now)
    day146097 = day146097 mod 146097
    Dim fs as FruehsportMode
    fs = BewegungsVorfestleg( day146097 , dataForM )
    ' FruehsportMode auf Keiner und FastImmer reduzieren
    if FruehsportMode.Wetterfrage = fs AndAlso janein1("Regnet es oder ist es unter -5 Grad") then
      fs = FruehsportMode.Keiner
    Elseif FruehsportMode.Wetterfrage = fs then
      fs = FruehsportMode.FastImmer
    End if
    Console.WriteLine(VbLf)
    If DateTime.Now.Hour >= 7 AndAlso DateTime.Today.DayOfWeek = DayOfWeek.Monday Then
      filled2bis = filled2bis + 1
      todo2(filled2bis) = "ggf Spülmaschine ein"
    End If
    if fs = FruehsportMode.FastImmer Then
      Console.WriteLine("-----> " & "Heute Frühsport, dann")  
    Else
      filled2bis = filled2bis + 1
      todo2(filled2bis) = "Waschen"
    End If
    If todo3 IsNot Nothing Then 
      filled2bis = filled2bis + 1
      todo2(filled2bis) = todo3
    End If
    If dataForM.LowCarbTeil.Length() > 0 Then
      Dim ur as String = dataForM.LowCarbTeil( day146097 mod dataForM.LowCarbTeil.Length() )
      filled2bis = filled2bis + 1
      todo2(filled2bis) = If( len(ur)<23, "Ggf. " & ur, ur )     
    End If
    If fs <> FruehsportMode.FastImmer Then
      If DateTime.Today.DayOfWeek < dataForM.ZeitknapperAbTag OrElse DateTime.Today.DayOfWeek > dataForM.ZeitknapperBisTag Then
        filled2bis = filled2bis + 2
        todo2(filled2bis-1) = "Warmes Frühstück zubereiten"
        todo2(filled2bis) = "Kleidung auf Sauberkeit Prüfen"
      End If
      filled2bis = filled2bis + 1
      todo2(filled2bis) = If( dataForM.LowCarbTeil.Length() > 0 , "Haupt-Frühstück" , "Frühstück" )
    Else
      filled2bis = filled2bis + 1
      todo2(filled2bis) = If( day146097 And 1 > 0 , "Mini Kuchen-Muschel" , "Milchbrötchen einstecken" )
      ' Wenn man vor dem Frühstück Sport macht, sollte man trotzdem minimal Kohlenhydrate essen, da der Körper sonst Eiweiss verbrennt
    End If
    Dim work1 as LocationOfDayWork
    if dataForM.WhereSundayToSaturday.Length() <> 7 Then errorAbbruch( "WhereSundayToSaturday.Length()", Nothing )
    work1 = dataForM.WhereSundayToSaturday( day146097 mod 7 )
    Console.WriteLine( "-----> " & work1.ToString() ) 
    Console.WriteLine(VbLf)
    If dataForM.URL_ofDay.Length() > 0 Then
      Dim url as String = dataForM.URL_ofDay( day146097 mod dataForM.URL_ofDay.Length() )
      Console.WriteLine( "-----> " & url ) 
      filled2bis = filled2bis + 1
      todo2(filled2bis) = If( len(url)<27, url, "Bitte o.g. URL ansehen")     
      Try
          dim psi as new ProcessStartInfo(url)
          psi.UseShellExecute = true
          System.Diagnostics.Process.Start( psi )
      Catch ee As Exception
          errorAbbruch( url  , ee )
      End Try
    End if  
    If DateTime.Now.Hour < 7 AndAlso DateTime.Today.DayOfWeek = DayOfWeek.Monday Then
      filled2bis = filled2bis + 1
      todo2(filled2bis) = "ggf Spülmaschine ein"
    End If
    if fs = FruehsportMode.FastImmer Then
      filled2bis = filled2bis + 1
      todo2(filled2bis) = "Sportkleidung für Outdoor"     
    ElseIf work1 = LocationOfDayWork.EinkaufenUndHaushalt OrElse work1 = LocationOfDayWork.Externarbeit OrElse 
           work1=LocationOfDayWork.Fruehschicht OrElse work1=LocationOfDayWork.ExterneWeiterbildung Then
      filled2bis = filled2bis + 1
      todo2(filled2bis) = "Kleidung für Outdoor" 
    End If
  End Sub
 
 
  Function BewegungsVorfestleg( day146097 As Integer, regelstruct As DataForMorningP ) As FruehsportMode
    Dim laenge = regelstruct.FruehsportModeArray.Length()
    if 0=laenge Then Return FruehsportMode.FastImmer
    Dim fs as FruehsportMode
    fs = regelstruct.FruehsportModeArray( day146097 mod laenge )
    ' FruehsportMode auf Keiner, Wetterfrage und FastImmer reduzieren
    If FruehsportMode.Sommerhalbjahr = fs AndAlso DateTime.Today.Month >= 5 AndAlso DateTime.Today.Month <= 10 Then
      fs = FruehsportMode.FastImmer
    Elseif FruehsportMode.Sommerhalbjahr = fs then
      fs = FruehsportMode.Keiner
    Elseif FruehsportMode.WetterAbApril = fs AndAlso DateTime.Today.Month >= 4 then
      fs = FruehsportMode.Wetterfrage
    Elseif FruehsportMode.WetterAbApril = fs then
      fs = FruehsportMode.Keiner
    End If
    Return fs
  End Function

  Dim left() as String
  Dim r() as String

 
  Function janein1(text As String)
    Dim line As String
    Do
      Console.Write("{0} ? (j/n) ", text)
      line = Console.ReadKey(true).KeyChar 
    Loop Until "jnJN".Contains(line)
    If line.ToUpper() = "J" then 
      Console.Write(" Ja ")
    Else 
      Console.Write(" Nein ")
    End If
    Console.WriteLine(" ")
    Return line.ToUpper() = "J"
  End Function
  
  
  Private Sub dozweispaltigchecklist
  ' Private Sub dozweispaltigchecklist(left() As String,r() As String)
    Console.WriteLine( VbCrLf & "Bitte antworten, welche Seite ( L oder R ) abgearbeitet ist (e oder l / r)."& vbCrlf)
    Dim lindex as Integer = 1
    Dim rindex as Integer = 1
    Dim line As String
    Console.WriteLine("Es sind {0}+{1} Items abzuarbeiten." & VbCrLf & VbCrLf & VbCrLf  & VbCrLf & VbCrLf & VbCrLf, left.Length-1, r.Length-1)
    Dim spaceline = new String( " "c, 78)
    Do While lindex < left.Length OrElse rindex < r.Length
     Console.Write(vbcr &  spaceline )
     Do
      If lindex < left.length and rindex < r.length and len(left(lindex))>36 then
        Console.Write(vbcr &  "{0}, {1} ? ", left(lindex),r(rindex))
      ElseIf lindex < left.length and rindex < r.length then
        Console.Write(vbcr &  "{0},    {1} ?   ", left(lindex),r(rindex))
      Elseif lindex < left.length  and len(left(lindex))>39  then
        Console.Write(vbcr &  "{0},      nix ?  ", left(lindex))
      Elseif lindex < left.length then
        Console.Write(vbcr &  "{0},      nix ? ", left(lindex))
      Else
        Console.Write(vbcr &  "nix,      {0} ? ",  r(rindex))
      End if
      line = Console.ReadKey(true).KeyChar 
     Loop Until "eElLrR".Contains(line)
     If line.ToUpper() = "R" then 
      rindex  = rindex  + 1
     Else 
      lindex  = lindex  + 1
     End If
    Loop
    Console.WriteLine( VbCrLf & VbCrLf & "Fertig." & VbCrLf )
  End Sub


  Sub errorAbbruch( text1 as String, text2 as Exception )
      Console.WriteLine(text1)    
      if text2 IsNot Nothing Then Console.WriteLine(text2)
      Console.WriteLine("Please enter sth to end the program.")
      Console.ReadKey()
      Environment.Exit(-1)
  End Sub
End Module


Public Module Interf
  Public Enum LocationOfDayWork
    Heimarbeit = 0            '
    HaushaltUndHeimarbeit = 1 '
    EinkaufenUndHaushalt = 2  '
    Externarbeit = 3          '
    Frei = 4                  '
    Fruehschicht = 5          '
    Lernen = 6                '
    ExterneWeiterbildung = 7  '
  End Enum
  
  
  Public Enum FruehsportMode
    Keiner
    Sommerhalbjahr
    Wetterfrage
    WetterAbApril
    FastImmer
  End Enum

  Public Structure DataForMorningP
    Public WhereSundayToSaturday() As LocationOfDayWork REM Anzahl 7 kann ich hier nicht festlegen.
    Public URL_ofDay() As String
    Public LowCarbTeil()  As String
    Public ZeitknapperAbTag As Integer
    Public ZeitknapperBisTag As Integer
    Public DayOfLastWalkOrJogging As Integer
    Public FruehsportModeArray() As FruehsportMode
    Public WinterHeizAbMMDD  As Integer
    Public WinterHeizBisMDD  As Integer
  End Structure

  Public Structure WorteForMorningP
    Public VersionOfStruct As Decimal
    Public OftR() as String
  End Structure
End Module

Public Module GlobalConstants
    Public ReadOnly base_sunday As Date = New Date(2018, 9, 2) ' Muss ein Sonntag sein = Tag 0. Die Zshl der Tage ab dem 31.12.1899 ist durch 63 teilbar.
End Module
