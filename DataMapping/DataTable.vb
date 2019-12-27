Imports System.ComponentModel
Imports System.Reflection
Imports System.Linq.Expressions

Public Class DataTable(Of T As Item)
    Inherits TypedTableBase(Of T)

    Private Shared ReadOnly _type As Type = GetType(T),
        NewRowBuilder As Func(Of DataRowBuilder, T) = ConstructorGenerico()

    Private Shared Function ConstructorGenerico() As Func(Of DataRowBuilder, T)
        With Expression.Parameter(GetType(DataRowBuilder), "builder")
            Return Expression.Lambda(Of Func(Of DataRowBuilder, T))(Expression.[New](GetType(T).GetConstructors(BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic).First(), .Self), .Self).Compile
        End With
    End Function

    Shadows Function NewRow() As T
        Return MyBase.NewRow
    End Function

    Protected Shadows Function GetRowType() As Type
        Return GetType(T)
    End Function

    Protected Overrides Function NewRowFromBuilder(builder As DataRowBuilder) As Data.DataRow
        Return NewRowBuilder(builder)
    End Function

    Public Class Item
        Inherits DataRow

        Protected Friend Sub New(dr As DataRowBuilder)
            MyBase.New(dr)
        End Sub

        Shadows ReadOnly Property Table As DataTable(Of T)
            Get
                Return MyBase.Table
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return ToString(DataRowVersion.Current)
        End Function

    End Class

End Class