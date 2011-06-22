        #region Factory Methods
<%
if (UseBoth())
{
    %>

#if !SILVERLIGHT
<%
}
if (UseNoSilverlight())
{
    %>
<!-- #include file="NewObject.asp" -->
<!-- #include file="NewObjectAsync.asp" -->
<%
    bool selfLoad2 = GetSelfLoad(Info);
    bool lazyLoad2 = GetLazyLoad(Info);
    bool isCollection = false;
    if (Info.DbName != String.Empty)
    {
        CslaObjectInfo tmpInfo = Info.Parent.CslaObjects.Find(Info.ParentType);
        if (tmpInfo != null)
            isCollection = IsCollectionType(tmpInfo.ObjectType);
        if (selfLoad2 && lazyLoad2 && !isCollection)
        {
            %>
<!-- #include file="GetObject.asp" -->
<%
            if (CurrentUnit.GenerationParams.SilverlightUsingServices)
            {
                %>
<!-- #include file="GetObjectAsync.asp" -->
<%
            }
        }
        else if (UseNoSilverlight())
        {
            %>
<!-- #include file="InternalGetObject.asp" -->
<%
        }
    }
}
//if (UseBoth() && HasFactoryCreateOrGet(Info))
if (UseBoth())
{
    %>

#else
<%
}
%>
<!-- #include file="NewObjectSilverlight.asp" -->
<!-- #include file="GetObjectSilverlight.asp" -->
<%
if (UseBoth())
{
    %>

#endif
<%
}
if (!CurrentUnit.GenerationParams.SilverlightUsingServices)
{
    %>
<!-- #include file="GetObjectAsync.asp" -->
<%
}
%>

        #endregion