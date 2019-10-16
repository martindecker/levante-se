Imports System
Imports System.Xml.Serialization
Imports System.IO

Module Program
    Public todo3 = VbLf

    Sub Main(args As String())
        LoadAData()
        Dim stoerungen2 = If( False, False, janein1("Straße arg befahren"))
        If stoerungen2 AndAlso DateTime.Today.DayOfWeek = DayOfWeek.Saturday Then
            todo3 = todo3 & "Ursache fuer stark befahrene Straße herausfinden"
        Else
            Console.WriteLine("nix")
        End If
        Console.WriteLine(VbLf)
		Planungsfragen()
        Console.WriteLine("Please enter sth to end the program.")
        Console.ReadKey()
    End Sub
    
  Dim dataForM as DataForMorningP
  Dim wForM    as WorteForMorningP

  Private Sub LoadAData()
    Dim fn = "jsonxmlm/daten_fuer_morgen.xml"
    Try
      For Teste = 1 To 5
        If not File.Exists(fn) Then fn = "../" & fn
      Next
      Dim stm As FileStream = New FileStream(fn, FileMode.Open)
      Dim xmlSer As XmlSerializer = New XmlSerializer(GetType(DataForMorningP))
      dataForM = CType(xmlSer.Deserialize(stm), DataForMorningP)
      stm.Close()    ' Testprint waere:   Console.WriteLine(dataForM.DayOfLastWalkOrJogging)
      fn = "jsonxmlm/worte_fuer_morgen.xml"
      For Teste = 1 To 5
        If not File.Exists(fn) Then fn = "../" & fn
      Next
      stm = New FileStream(fn, FileMode.Open)
      xmlSer = New XmlSerializer(GetType(WorteForMorningP))
      wForM = CType(xmlSer.Deserialize(stm), WorteForMorningP)
      stm.Close()    ' Testprint waere:   Console.WriteLine(wForM.VersionOfStruct)
    Catch eee As Exception  
      errorAbbruch(fn & "   Error:", eee)
    End Try 
  End Sub
  
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
    if fs = FruehsportMode.FastImmer Then Console.WriteLine("-----> " & "Heute Frühsport, dann")  
	Dim work1 as LocationOfDayWork
	if dataForM.WhereSundayToSaturday.Length() <> 7 Then errorAbbruch( "WhereSundayToSaturday.Length()", Nothing )
	work1 = dataForM.WhereSundayToSaturday( day146097 mod 7 )
	Console.WriteLine( "-----> " & work1.ToString() ) 
    Console.WriteLine(VbLf)
    If dataForM.URL_ofDay.Length() > 0 Then
	  Dim url as String = dataForM.URL_ofDay( day146097 mod dataForM.URL_ofDay.Length() )
	  Console.WriteLine( "-----> " & url ) 
    End if	
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

 
  Private Sub dozweispaltigchecklist(left() As String,r() As String)
    Console.WriteLine("Bitte antworten, welche Seite ( L oder R ) abgearbeitet ist (l/r)"& vbCrlf)
    Dim lindex as Integer = 1
    Dim rindex as Integer = 1
    Dim line As String
    Do While lindex < len(left) or rindex < len(r) 
     Do
      If lindex < len(left) and rindex < len(r) then
        Console.Write(vbcr &  "{0},[1] ?       ", left(lindex),r(rindex))
      Elseif lindex < len(left) then
        Console.Write(vbcr &  "{0}, nix ?       ", left(lindex))
      Else
        Console.Write(vbcr &  "nix,[1] ?       ",  r(rindex))
      End if
      line = Console.ReadKey(true).KeyChar 
     Loop Until "lLrR".Contains(line)
     If line.ToUpper() = "R" then 
      rindex  = rindex  + 1
     Else 
      lindex  = lindex  + 1
     End If
    Loop
    Console.WriteLine("Fertig ")
  End Sub


  Sub errorAbbruch( text1 as String, text2 as Exception )
      Console.WriteLine(text1)	  
      if text2 IsNot Nothing Then Console.WriteLine(text2)
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
    Public DayOfLastWalkOrJogging As Integer
    Public FruehsportModeArray() As FruehsportMode
  End Structure

  Public Structure WorteForMorningP
    Public VersionOfStruct As Decimal
    Public OftR() as String
  End Structure
End Module

Public Module GlobalConstants
    Public ReadOnly base_sunday As Date = New Date(2018, 9, 2) ' Muss ein Sonntag sein = Tag 0. Die Zshl der Tage ab dem 31.12.1899 ist durch 63 teilbar.
End Module
