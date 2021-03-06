'  This file was generated by CSLA Object Generator - CslaGenFork v4.5
'
' Filename:    CircTypeTagEditDynaColl
' ObjectType:  CircTypeTagEditDynaColl
' CSLAType:    DynamicEditableRootCollection

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports Csla
Imports Csla.Data
Imports DocStore.Business.Util
Imports UsingLibrary

Namespace DocStore.Business

    ''' <summary>
    ''' CircTypeTagEditDynaColl (dynamic root list).<br/>
    ''' This is a generated base class of <see cref="CircTypeTagEditDynaColl"/> business object.
    ''' </summary>
    ''' <remarks>
    ''' The items of the collection are <see cref="CircTypeTagEditDyna"/> objects.
    ''' </remarks>
    <Serializable>
    Public Partial Class CircTypeTagEditDynaColl
#If WINFORMS Then
        Inherits MyDynamicBindingListBase(Of CircTypeTagEditDyna)
        Implements IHaveInterface, IHaveGenericInterface(Of CircTypeTagEditDynaColl)
#Else
        Inherits MyDynamicListBase(Of CircTypeTagEditDyna)
        Implements IHaveInterface, IHaveGenericInterface(Of CircTypeTagEditDynaColl)
#End If

        #Region " Collection Business Methods "

        ''' <summary>
        ''' Removes a <see cref="CircTypeTagEditDyna"/> item from the collection.
        ''' </summary>
        ''' <param name="circTypeID">The CircTypeID of the item to be removed.</param>
        Public Overloads Sub Remove(circTypeID As Integer)
            For Each item As CircTypeTagEditDyna In Me
                If item.CircTypeID = circTypeID Then
                    MyBase.Remove(item)
                    Exit For
                End If
            Next
        End Sub

        ''' <summary>
        ''' Determines whether a <see cref="CircTypeTagEditDyna"/> item is in the collection.
        ''' </summary>
        ''' <param name="circTypeID">The CircTypeID of the item to search for.</param>
        ''' <returns><c>True</c> if the CircTypeTagEditDyna is a collection item; otherwise, <c>false</c>.</returns>
        Public Overloads Function Contains(circTypeID As Integer) As Boolean
            For Each item As CircTypeTagEditDyna In Me
                If item.CircTypeID = circTypeID Then
                    Return True
                End If
            Next
            Return False
        End Function

        #End Region

        #Region " Factory Methods "

        ''' <summary>
        ''' Factory method. Creates a new <see cref="CircTypeTagEditDynaColl"/> collection.
        ''' </summary>
        ''' <returns>A reference to the created <see cref="CircTypeTagEditDynaColl"/> collection.</returns>
        Public Shared Function NewCircTypeTagEditDynaColl() As CircTypeTagEditDynaColl
            Return DataPortal.Create(Of CircTypeTagEditDynaColl)()
        End Function

        ''' <summary>
        ''' Factory method. Loads a <see cref="CircTypeTagEditDynaColl"/> collection.
        ''' </summary>
        ''' <returns>A reference to the fetched <see cref="CircTypeTagEditDynaColl"/> collection.</returns>
        Public Shared Function GetCircTypeTagEditDynaColl() As CircTypeTagEditDynaColl
            Return DataPortal.Fetch(Of CircTypeTagEditDynaColl)()
        End Function

        ''' <summary>
        ''' Factory method. Asynchronously creates a new <see cref="CircTypeTagEditDynaColl"/> collection.
        ''' </summary>
        ''' <param name="callback">The completion callback method.</param>
        Public Shared Sub NewCircTypeTagEditDynaColl(callback As EventHandler(Of DataPortalResult(Of CircTypeTagEditDynaColl)))
            DataPortal.BeginCreate(Of CircTypeTagEditDynaColl)(callback)
        End Sub

        ''' <summary>
        ''' Factory method. Asynchronously loads a <see cref="CircTypeTagEditDynaColl"/> collection.
        ''' </summary>
        ''' <param name="callback">The completion callback method.</param>
        Public Shared Sub GetCircTypeTagEditDynaColl(ByVal callback As EventHandler(Of DataPortalResult(Of CircTypeTagEditDynaColl)))
            DataPortal.BeginFetch(Of CircTypeTagEditDynaColl)(callback)
        End Sub

        #End Region

        #Region " Constructor "

        ''' <summary>
        ''' Initializes a new instance of the <see cref="CircTypeTagEditDynaColl"/> class.
        ''' </summary>
        ''' <remarks> Do not use to create a Csla object. Use factory methods instead.</remarks>
        <System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>
        Public Sub New()
            ' Use factory methods and do not use direct creation.

            Dim rlce = RaiseListChangedEvents
            RaiseListChangedEvents = False
            AllowNew = True
            AllowEdit = True
            AllowRemove = True
            RaiseListChangedEvents = rlce
        End Sub

        #End Region

        #Region " Data Access "

        ''' <summary>
        ''' Loads a <see cref="CircTypeTagEditDynaColl"/> collection from the database.
        ''' </summary>
        Protected Overloads Sub DataPortal_Fetch()
            Using ctx = ConnectionManager(Of SqlConnection).GetManager(Database.DocStoreConnection, False)
                Using cmd = New SqlCommand("GetCircTypeTagEditDynaColl", ctx.Connection)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim args As New DataPortalHookArgs(cmd)
                    OnFetchPre(args)
                    LoadCollection(cmd)
                    OnFetchPost(args)
                End Using
            End Using
        End Sub

        Private Sub LoadCollection(cmd As SqlCommand)
            Using dr As New SafeDataReader(cmd.ExecuteReader())
                Fetch(dr)
            End Using
        End Sub

        ''' <summary>
        ''' Loads all <see cref="CircTypeTagEditDynaColl"/> collection items from the given SafeDataReader.
        ''' </summary>
        ''' <param name="dr">The SafeDataReader to use.</param>
        Private Sub Fetch(dr As SafeDataReader)
            Dim rlce = RaiseListChangedEvents
            RaiseListChangedEvents = False
            While dr.Read()
                Add(CircTypeTagEditDyna.GetCircTypeTagEditDyna(dr))
            End While
            RaiseListChangedEvents = rlce
        End Sub

        #End Region

        #Region " DataPortal Hooks "

        ''' <summary>
        ''' Occurs after setting query parameters and before the fetch operation.
        ''' </summary>
        Partial Private Sub OnFetchPre(args As DataPortalHookArgs)
        End Sub

        ''' <summary>
        ''' Occurs after the fetch operation (object or collection is fully loaded and set up).
        ''' </summary>
        Partial Private Sub OnFetchPost(args As DataPortalHookArgs)
        End Sub

        #End Region

    End Class
End Namespace
