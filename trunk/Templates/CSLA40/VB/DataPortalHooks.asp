<%
if ((Info.IsEditableRoot() ||
    Info.IsEditableChild() ||
    Info.IsDynamicEditableRoot() ||
    Info.IsEditableSwitchable()) &&
    Info.SupportUpdateProperties == true)
{
    Infos.Append("To do list: edit \"" + Info.ObjectName + ".vb\", uncomment the \"OnDeserialized\" method and add the following line:" + Environment.NewLine);
    Infos.Append("      AddHandler Saved, AddressOf On" + Info.ObjectName + "Saved" + Environment.NewLine);
    %>
        #Region " Saved Event "
<%
        if (CurrentUnit.GenerationParams.WriteTodo)
        {
            %>

        'TODO: edit "<%= Info.ObjectName %>.vb", uncomment the "OnDeserialized" method and add the following line:
        'TODO:     AddHandler Saved, AddressOf On<%= Info.ObjectName %>Saved
<%
        }
        %>

        Private Sub On<%= Info.ObjectName %>Saved(sender As Object, e As Csla.Core.SavedEventArgs)
                RaiseEvent <%= Info.ObjectName %>Saved(sender, e)
        End Sub

        ''' <summary> Use this event to signal a <see cref="<%= Info.ObjectName %>"/> object was saved.</summary>
        Public Shared Event <%= Info.ObjectName %>Saved As EventHandler(Of Csla.Core.SavedEventArgs)

        #End Region

<%
}
System.Collections.Generic.List<string> hookList = GetHookList(Info);
if (hookList.Count > 0 && UseNoSilverlight())
{
    %>
        #Region " DataPortal Hooks "
<%
    if (UseBoth() && !HasSilverlightLocalDataPortalCreate(Info))
    {
        %>

#If Not SILVERLIGHT Then
<%
    }
    foreach (string hookName in hookList)
    {
    %>

        ''' <summary>
        ''' Occurs <%= FormatHookDocumentation(hookName) %>
        ''' </summary>
        Partial Private Sub On<%= hookName %>(args As DataPortalHookArgs)
        End Sub
        <%
        if (hookName == "Create" && HasSilverlightLocalDataPortalCreate(Info))
        {
            %>

#If Not SILVERLIGHT Then
        <%
        }
    }
    if (UseBoth())
    {
        %>

#End If
<%
    }
%>

        #End Region

<%
}
%>
