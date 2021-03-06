using System;
using System.Collections.Generic;
using Csla;
using Invoices.DataAccess;

namespace Invoices.Business
{

    /// <summary>
    /// InvoiceLineList (read only list).<br/>
    /// This is a generated <see cref="InvoiceLineList"/> business object.
    /// </summary>
    /// <remarks>
    /// This class is child of <see cref="InvoiceView"/> read only object.<br/>
    /// The items of the collection are <see cref="InvoiceLineInfo"/> objects.
    /// </remarks>
    [Serializable]
#if WINFORMS
    public partial class InvoiceLineList : ReadOnlyBindingListBase<InvoiceLineList, InvoiceLineInfo>
#else
    public partial class InvoiceLineList : ReadOnlyListBase<InvoiceLineList, InvoiceLineInfo>
#endif
    {

        #region Collection Business Methods

        /// <summary>
        /// Determines whether a <see cref="InvoiceLineInfo"/> item is in the collection.
        /// </summary>
        /// <param name="invoiceLineId">The InvoiceLineId of the item to search for.</param>
        /// <returns><c>true</c> if the InvoiceLineInfo is a collection item; otherwise, <c>false</c>.</returns>
        public bool Contains(Guid invoiceLineId)
        {
            foreach (var invoiceLineInfo in this)
            {
                if (invoiceLineInfo.InvoiceLineId == invoiceLineId)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceLineList"/> class.
        /// </summary>
        /// <remarks> Do not use to create a Csla object. Use factory methods instead.</remarks>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public InvoiceLineList()
        {
            // Use factory methods and do not use direct creation.

            var rlce = RaiseListChangedEvents;
            RaiseListChangedEvents = false;
            AllowNew = false;
            AllowEdit = false;
            AllowRemove = false;
            RaiseListChangedEvents = rlce;
        }

        #endregion

        #region Data Access

        /// <summary>
        /// Loads all <see cref="InvoiceLineList"/> collection items from the given list of InvoiceLineInfoDto.
        /// </summary>
        /// <param name="data">The list of <see cref="InvoiceLineInfoDto"/>.</param>
        private void Child_Fetch(List<InvoiceLineInfoDto> data)
        {
            IsReadOnly = false;
            var rlce = RaiseListChangedEvents;
            RaiseListChangedEvents = false;
            var args = new DataPortalHookArgs(data);
            OnFetchPre(args);
            foreach (var dto in data)
            {
                Add(DataPortal.FetchChild<InvoiceLineInfo>(dto));
            }
            OnFetchPost(args);
            RaiseListChangedEvents = rlce;
            IsReadOnly = true;
        }

        #endregion

        #region DataPortal Hooks

        /// <summary>
        /// Occurs after setting query parameters and before the fetch operation.
        /// </summary>
        partial void OnFetchPre(DataPortalHookArgs args);

        /// <summary>
        /// Occurs after the fetch operation (object or collection is fully loaded and set up).
        /// </summary>
        partial void OnFetchPost(DataPortalHookArgs args);

        #endregion

    }
}
