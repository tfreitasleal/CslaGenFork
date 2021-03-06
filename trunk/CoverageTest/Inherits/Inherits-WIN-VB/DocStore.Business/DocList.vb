'  This file was generated by CSLA Object Generator - CslaGenFork v4.5
'
' Filename:    DocList
' ObjectType:  DocList
' CSLAType:    ReadOnlyCollection

Imports System
Imports Csla
Imports DocStore.Business.Util

Namespace DocStore.Business

    Public Partial Class DocList

        #Region " OnDeserialized actions "

        ''' <summary>
        ''' This method is called on a newly deserialized object
        ''' after deserialization is complete.
        ''' </summary>
        Protected Overrides Sub OnDeserialized()
            MyBase.OnDeserialized()
            DocSaved.Register(Me)
            ' add your custom OnDeserialized actions here.
        End Sub

        #End Region

        #Region " Inline queries "

        ' Private Sub GetQueryGetDocList()
        '     getDocListInlineQuery = ""
        ' End Sub

        ' Private Sub GetQueryGetDocList(crit As DocListFilteredCriteria)
        '     getDocListInlineQuery = ""
        ' End Sub

        #End Region

        #Region " Implementation of DataPortal Hooks "

        ' Private Sub OnFetchPre(args As DataPortalHookArgs)
        '     Throw New NotImplementedException()
        ' End Sub

        ' Private Sub OnFetchPost(args As DataPortalHookArgs)
        '     Throw New NotImplementedException()
        ' End Sub

        #End Region

    End Class

    #Region " Criteria Object "

    Public Partial Class DocListFilteredCriteria

    End Class

    #End Region


End Namespace
