<%
if (!Info.UseCustomLoading)
{
    string fetchString = string.Empty;
    string methodFetchString = string.Empty;
    if (TypeHelper.IsNotRootType(Info) && !UseChildFactoryHelper)
        methodFetchString = "Child_";
    if (Info.IsDynamicEditableRoot() && !UseChildFactoryHelper)
        methodFetchString = "DataPortal_";

    if (!Info.DataSetLoadingScheme)
    {
        %>

        /// <summary>
        /// Loads a <see cref="<%= Info.ObjectName %>"/> object from the given SafeDataReader.
        /// </summary>
        /// <param name="dr">The SafeDataReader to use.</param>
        private void <%= methodFetchString %>Fetch(SafeDataReader dr)
        {
            // Value properties
            <%
        foreach (ValueProperty prop in Info.GetAllValueProperties())
        {
            if (prop.IsDatabaseBound &&
                prop.DbBindColumn.ColumnOriginType != ColumnOriginType.None &&
                prop.DataAccess != ValueProperty.DataAccessBehaviour.WriteOnly)
            {
                try
                {
                    %><%= GetReaderAssignmentStatement(Info, prop) %>;
            <%
                }
                catch (Exception ex)
                {
                    Errors.Append(ex.Message + Environment.NewLine);
                }
            }
        }

        bool isRoot = TypeHelper.IsRootType(Info);
        bool isRootLoader = (ancestorLoaderLevel == 0 && !ancestorIsCollection) || (ancestorLoaderLevel == 1 && ancestorIsCollection);

        // parent loading field
        if (useFieldForParentLoading)
        {
            %>// parent properties
            <%
            foreach (Property prop in Info.ParentProperties)
            {
                %><%= FormatCamel(GetFKColumn(Info, (isItem ? grandParentInfo : parentInfo), prop)) %> = dr.<%= GetReaderMethod(prop.PropertyType) %>("<%= GetFKColumn(Info, (isItem ? grandParentInfo : parentInfo), prop) %>");
            <%
            }
        }
        // state property
        if (useIsLoadedProperty)
        {
            %>// State property
            LoadProperty(IsLoadedProperty, true);
            <%
        }
            %>var args = new DataPortalHookArgs(dr);
            OnFetchRead(args);
        <%
        if (ancestorLoaderLevel > 0 && !UseChildFactoryHelper)
        {
            foreach (ChildProperty childProp in Info.GetCollectionChildProperties())
            {
                CslaObjectInfo _child = FindChildInfo(Info, childProp.TypeName);
                if (_child != null)
                {
                    if (childProp.LoadingScheme == LoadingScheme.ParentLoad)
                    {
                        string internalCreateString = string.Empty;
                        if (TypeHelper.IsEditableType(_child.ObjectType))
                        {
                            if (UseChildFactoryHelper)
                                internalCreateString = FormatPascal(childProp.TypeName) + ".New" + FormatPascal(childProp.TypeName);
                            else
                                internalCreateString = "DataPortal.CreateChild<" + FormatPascal(childProp.TypeName) + ">";
                        }
                        else
                            internalCreateString = "new " + childProp.TypeName;

                        if ((childProp.DeclarationMode == PropertyDeclaration.Managed ||
                            childProp.DeclarationMode == PropertyDeclaration.ManagedWithTypeConversion))
                        {
                            %>
            LoadProperty(<%= FormatPropertyInfoName(childProp.Name) %>, <%= internalCreateString %>());
        <%
                        }
                        else
                        {
                            %>
            <%= bpcSpacer %><%= GetFieldLoaderStatement(childProp, internalCreateString + "()") %>;
        <%
                        }
                    }
                }
            }
        }
        if (!UseChildFactoryHelper && Info.CheckRulesOnFetch && !Info.EditOnDemand && (!isRoot || Info.IsDynamicEditableRoot()))
        {
            %>
            // check all object rules and property rules
            BusinessRules.CheckRules();
        <%
        }
        %>
        }
        <%
        if (ParentLoadsChildren(Info))
        {
            if (isRootLoader)
            {
                %>

        /// <summary>
        /// Loads child objects from the given SafeDataReader.
        /// </summary>
        /// <param name="dr">The SafeDataReader to use.</param>
        <%= isRoot ? "private" : "internal" %> void FetchChildren(SafeDataReader dr)
        {
            <%
                foreach (ChildProperty childProp in Info.GetAllChildProperties())
                {
                    if (childProp.LoadingScheme == LoadingScheme.ParentLoad)
                    {
                        CslaObjectInfo _child = FindChildInfo(Info, childProp.TypeName);
                        if (_child != null)
                        {
                            %>
            <%= bpcSpacer %>dr.NextResult();
<%
                            if (UseChildFactoryHelper)
                                fetchString = FormatPascal(childProp.TypeName) + ".Get" + FormatPascal(childProp.TypeName);
                            else
                                fetchString = "DataPortal.FetchChild<" + FormatPascal(childProp.TypeName) + ">";

                            if (TypeHelper.IsCollectionType(_child.ObjectType))
                            {
                                if (ancestorLoaderLevel == 1 && ancestorIsCollection)
                                {
                                    CslaObjectInfo child = FindChildInfo(Info, childProp.TypeName);
                                    if (child != null)
                                    {
                                        ChildProperty ancestorChildProperty = new ChildProperty();
                                        CslaObjectInfo _parent = child.FindMyParent(child);
                                        if (_parent != null)
                                        {
                                            %>
            <%= bpcSpacer %>var <%= FormatCamel(childProp.TypeName) %> = <%= fetchString %>(dr);
<%
                                            if (child.IsReadOnlyCollection())
                                            {
                                                %>
            <%= bpcSpacer %><%= FormatCamel(childProp.TypeName) %>.LoadItems(ParentList);
<%
                                            }
                                            else
                                            {
                                                %>
            <%= bpcSpacer %><%= FormatCamel(childProp.TypeName) %>.LoadItems((<%= FormatPascal(_parent.ParentType) %>)Parent);
<%
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if ((childProp.DeclarationMode == PropertyDeclaration.Managed ||
                                        childProp.DeclarationMode == PropertyDeclaration.ManagedWithTypeConversion))
                                    {
                                        %>
            LoadProperty(<%= FormatPropertyInfoName(childProp.Name) %>, <%= fetchString %>(dr));
        <%
                                    }
                                    else
                                    {
                                        %>
            <%= bpcSpacer %><%= GetFieldLoaderStatement(childProp, fetchString + "(dr)") %>;
        <%
                                    }
                                }
                            }
                            else if (ancestorLoaderLevel == 1 && ancestorIsCollection)
                            {
                                string findByParams = string.Empty;
                                bool firstFind = true;
                                foreach (Property prop in _child.ParentProperties)
                                {
                                    if (firstFind)
                                        firstFind = false;
                                    else
                                        findByParams += ", ";

                                    findByParams += "child." + FormatCamel(GetFKColumn(_child, Info, prop));
                                }
                                CslaObjectInfo child = FindChildInfo(Info, childProp.TypeName);
                                if (child != null)
                                {
                                    %>
            <%= bpcSpacer %>while (dr.Read())
            <%= bpcSpacer %>{
                <%= bpcSpacer %>var child = <%= fetchString %>(dr);
<%
                                    if (child.IsReadOnlyObject())
                                    {
                                        %>
                <%= bpcSpacer %>var obj = ParentList.Find<%= FormatPascal(Info.ObjectName) %>ByParentProperties(<%= findByParams %>);
<%
                                    }
                                    else
                                    {
                                        %>
                <%= bpcSpacer %>var obj = ((<%= Info.ParentType %>)Parent).Find<%= FormatPascal(Info.ObjectName) %>ByParentProperties(<%= findByParams %>);
<%
                                    }
                                    if (child.IsReadOnlyObject() && child.AddParentReference)
                                    {
                                        %>
                <%= bpcSpacer %>child.ParentList = obj;
<%
                                    }
                                    if (childProp.DeclarationMode == PropertyDeclaration.Managed ||
                                        childProp.DeclarationMode == PropertyDeclaration.ManagedWithTypeConversion)
                                    {
                                        %>
                obj.LoadProperty(<%= FormatPropertyInfoName(childProp.Name) %>, child);
            <%
                                    }
                                    else
                                    {
                                        %>
                obj.<%= GetFieldLoaderStatement(childProp, "child") %>;
            <%
                                    }
                                }
                                %>
            <%= bpcSpacer %>}
<%
                            }
                            else
                            {
                                %>
            <%= bpcSpacer %>if (dr.Read())
<%
                                CslaObjectInfo child = FindChildInfo(Info, childProp.TypeName);
                                if (child != null)
                                {
                                    if (child.IsReadOnlyObject() && child.AddParentReference)
                                    {
                                        %>
            <%= bpcSpacer %>{
                <%= bpcSpacer %>var child = <%= fetchString %>(dr);
                <%= bpcSpacer %>child.Parent = this;
<%
                                        if (childProp.DeclarationMode == PropertyDeclaration.Managed ||
                                            childProp.DeclarationMode == PropertyDeclaration.ManagedWithTypeConversion)
                                        {
                                            %>
                LoadProperty(<%= FormatPropertyInfoName(childProp.Name) %>, child);
            <%
                                        }
                                        else
                                        {
                                            %>
                <%= GetFieldLoaderStatement(childProp, "child") %>;
            <%
                                        }
                                        %>
            <%= bpcSpacer %>}
            <%
                                    }
                                    else
                                    {
                                        if (childProp.DeclarationMode == PropertyDeclaration.Managed ||
                                            childProp.DeclarationMode == PropertyDeclaration.ManagedWithTypeConversion)
                                        {
                                            %>
                LoadProperty(<%= FormatPropertyInfoName(childProp.Name) %>, <%= fetchString %>(dr));
            <%
                                        }
                                        else
                                        {
                                            %>
                <%= GetFieldLoaderStatement(childProp, fetchString + "(dr)") %>;
            <%
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (ChildProperty childProp in GetParentLoadAllGrandChildPropertiesInHierarchy(Info, true))
                {
                    if (childProp.LoadingScheme == LoadingScheme.ParentLoad)
                    {
                        bool childAncestorIsCollection = false;
                        int childAncestorLoaderLevel = 0;
                        CslaObjectInfo _child = FindChildInfo(Info, childProp.TypeName);
                        if (_child != null)
                        {
                            if (UseChildFactoryHelper)
                                fetchString = FormatPascal(childProp.TypeName) + ".Get" + FormatPascal(childProp.TypeName);
                            else
                                fetchString = "DataPortal.FetchChild<" + FormatPascal(childProp.TypeName) + ">";

                            childAncestorLoaderLevel = AncestorLoaderLevel(_child, out childAncestorIsCollection);
                            ChildProperty ancestorChildProperty = new ChildProperty();
                            CslaObjectInfo _parent = _child.FindMyParent(_child);
                            if (childAncestorLoaderLevel < 4 && _parent != null)
                            {
                                CslaObjectInfo _ancestor = _child.FindMyParent(_parent);
                                if (_ancestor != null)
                                    GetChildPropertyByTypeName(_ancestor, _parent.ParentType, ref ancestorChildProperty);
                            }
                            %>
            <%= bpcSpacer %>dr.NextResult();
            <%
                            if (TypeHelper.IsCollectionType(_child.ObjectType))
                            {
                                %>
            <%= bpcSpacer %>var <%= FormatCamel(childProp.TypeName) %> = <%= fetchString %>(dr);
            <%= bpcSpacer %><%= FormatCamel(childProp.TypeName) %>.LoadItems(<%= childAncestorLoaderLevel < 4 ? FormatPascal(ancestorChildProperty.Name) : FormatCamel(_parent.ParentType) %>);
<%
                            }
                            else
                            {
                                string findByParams = string.Empty;
                                bool firstFind = true;
                                foreach (Property prop in _child.ParentProperties)
                                {
                                    if (firstFind)
                                        firstFind = false;
                                    else
                                        findByParams += ", ";

                                    findByParams += "child." + FormatCamel(GetFKColumn(_child, _parent, prop));
                                }
                                string findByObject = string.Empty;
                                if (childAncestorLoaderLevel < 4)
                                    findByObject = FormatPascal(ancestorChildProperty.Name);
                                else
                                    findByObject = FormatCamel(_parent.ParentType);

                                %>
            <%= bpcSpacer %>while (dr.Read())
            <%= bpcSpacer %>{
                <%= bpcSpacer %>var child = <%= fetchString %>(dr);
                <%= bpcSpacer %>var obj = <%= findByObject %>.Find<%= FormatPascal(_parent.ObjectName) %>ByParentProperties(<%= findByParams %>);
<%
                                    if (_child.IsReadOnlyObject() && _child.AddParentReference)
                                    {
                                        %>
                <%= bpcSpacer %>child.Parent = obj;
<%
                                    }
                                    %>
                <%= bpcSpacer %>obj.LoadChild(child);
            <%= bpcSpacer %>}
        <%
                            }
                        }
                    }
                }

                %>
        }
        <%
            }
            else // !isRootLoader
            {
                foreach (ChildProperty childProp in Info.GetNonCollectionChildProperties())
                {
                    if (childProp.LoadingScheme == LoadingScheme.ParentLoad)
                    {
                        CslaObjectInfo _child = FindChildInfo(Info, childProp.TypeName);
                        if (_child != null)
                        {
                        %>

        /// <summary>
        /// Loads child <see cref="<%= FormatPascal(childProp.TypeName) %>"/> object.
        /// </summary>
        /// <param name="child">The child object to load.</param>
        internal void LoadChild(<%= FormatPascal(childProp.TypeName) %> child)
        {
            LoadProperty(<%= FormatPropertyInfoName(childProp.Name) %>, child);
        }
        <%
                        }
                    }
                }
            }
        }

        if (SelfLoadsChildren(Info))
        {
            %>

        /// <summary>
        /// Loads child objects.
        /// </summary>
        <%= isRoot ? "private" : "internal" %> void FetchChildren()
        {
            <%
            foreach (ChildProperty childProp in Info.GetMyChildProperties())
            {
                if (childProp.LoadingScheme == LoadingScheme.SelfLoad && !childProp.LazyLoad)
                {
                    CslaObjectInfo childInfo = FindChildInfo(Info, childProp.TypeName);
                    if (childInfo != null)
                    {
                        string invokeParam = string.Empty;
                        bool first1 = true;
                        //if (isParentRootCollection)
                        if (isItem)
                        {
                            foreach (Property prop in childProp.ParentLoadProperties)
                            {
                                if (first1)
                                    first1 = false;
                                else
                                    invokeParam += ", ";

                                invokeParam += FormatPascal(prop.Name);
                            }
                        }
                        else
                        {
                            foreach (Parameter parm in childProp.LoadParameters)
                            {
                                if (first1)
                                    first1 = false;
                                else
                                    invokeParam += ", ";

                                invokeParam += FormatPascal(parm.PropertyName);
                            }
                        }
                        if (UseChildFactoryHelper)
                            fetchString = FormatPascal(childProp.TypeName) + ".Get" + FormatPascal(childProp.TypeName);
                        else
                            fetchString = "DataPortal.FetchChild<" + FormatPascal(childProp.TypeName) + ">";

                        if (childProp.DeclarationMode == PropertyDeclaration.Managed)
                        {
                            %>
            <%= bpcSpacer %>LoadProperty(<%= FormatPropertyInfoName(childProp.Name) %>, <%= fetchString %>(<%= invokeParam %>));
            <%
                        }
                        else if (childProp.DeclarationMode == PropertyDeclaration.ClassicProperty ||
                            childProp.DeclarationMode == PropertyDeclaration.AutoProperty)
                        {
                            %>
            <%= bpcSpacer %><%= GetFieldLoaderStatement(childProp, fetchString +"(" + invokeParam + ")") %>;
            <%
                        }
                    }
                }
            }
            %>
        }
        <%
        }
    }
    else
    {
        %>

        /// <summary>
        /// Loads a <see cref="<%= Info.ObjectName %>"/> object from the given DataRow.
        /// </summary>
        /// <param name="dr">The DataRow to use.</param>
        private void <%= methodFetchString %>Fetch(DataRow dr)
        {
            // Value properties
            <%
            foreach (ValueProperty prop in Info.GetAllValueProperties())
            {
                if (prop.IsDatabaseBound &&
                    prop.DbBindColumn.ColumnOriginType != ColumnOriginType.None &&
                    prop.DataAccess != ValueProperty.DataAccessBehaviour.WriteOnly)
                {
                    %>if (!dr.IsNull("<%= prop.ParameterName %>"))
                <%= GetReaderAssignmentStatement(Info, prop) %>;
            <%
                }
            }
            %>var args = new DataPortalHookArgs(dr);
            OnFetchRead(args);
        }
        <%
        if (ParentLoadsChildren(Info))
        {
            %>

        /// <summary>
        /// Loads child objects using given DataRow.
        /// </summary>
        /// <param name="dr">The DataRow to use.</param>
        private void FetchChildren(DataRow dr)
        {
            DataRow[] childRows;
            <%
            foreach (ChildProperty childProp in Info.GetNonCollectionChildProperties())
            {
                if (childProp.LoadingScheme == LoadingScheme.ParentLoad)
                {
                    %>
            childRows = dr.GetChildRows("<%= Info.ObjectName + childProp.TypeName %>");
            if (childRows.Length > 0)
                <%= FormatFieldName(childProp.Name) %> = DataPortal.FetchChild<<%= childProp.TypeName %>>(childRows[0]);
            <%
                }
            }
            foreach (ChildProperty childProp in Info.GetCollectionChildProperties())
            {
                if (childProp.LoadingScheme == LoadingScheme.ParentLoad)
                {
                    %>
            childRows = dr.GetChildRows("<%= Info.ObjectName + FindChildInfo(Info, childProp.TypeName).ItemType %>");
            <%
                    CslaObjectInfo childInfo = FindChildInfo(Info, childProp.TypeName);
                    if (childInfo != null)
                    {
                        if (UseChildFactoryHelper)
                            fetchString = FormatPascal(childProp.TypeName) + ".Get" + FormatPascal(childProp.TypeName);
                        else
                            fetchString = "DataPortal.FetchChild<" + FormatPascal(childProp.TypeName) + ">";

                        %>
            <%= FormatFieldName(childProp.Name) %> = <%= fetchString %>(childRows);
            <%
                    }
                }
            }
            %>
        }
        <%
        }
    }
}
%>
