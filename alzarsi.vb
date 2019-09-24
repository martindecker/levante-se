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
        Console.WriteLine(todo3)
        Console.WriteLine("I began the project on August 22, 2018. Please enter sth to end the program.")
        Console.ReadKey()
    End Sub
	
  Dim dataForM as DataForMorningP

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
    Catch eee As Exception
      Console.WriteLine(fn & "   Error:")    
	  Console.WriteLine(eee)
	  Environment.Exit(-1)
    End Try	
  End Sub
  
  Function janein1(text As String)
    Dim line As String
    Do
      Console.Write("{0} ? (j/n) ", text)
      line = Console.ReadLine()
    Loop Until "jnJN".Contains(line)
    Return line.ToUpper() = "J"
  End Function

End Module

Public Module Interf
  Public Enum LocationOfDayWork
    Heimarbeit = 0            '
    HaushaltUndHeimarbeit = 1 '
    EinkaufenUndHaushalt = 2  '
    Externarbeit = 3          '
    Frei = 4                  '
    Fruehschicht = 5          '
    ExterneWeiterbildung = 6  '
    Lernen = 7                '
  End Enum
  
  Public Enum FruehsportMode
    Keiner
	Sommerhalbjahr
	Wetterfrage
	WetterAbApril
	FastImmer
  End Enum
  
  Public Structure DataForMorningP
    Public WhereSundayToSaturday() As LocationOfDayWork REM Anzahl kann ich hier nicht festlegen.
    Public DayOfLastWalkOrJogging As Integer
    Public FruehsportModeArray() As FruehsportMode
  End Structure
  
End Module

