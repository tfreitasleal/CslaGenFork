using System;
using System.Collections.Generic;
using Csla;
using Invoices.DataAccess;

namespace Invoices.Business
{

    /// <summary>
    /// InvoiceList (read only list).<br/>
    /// This is a generated <see cref="InvoiceList"/> business object.
    /// This class is a root collection.
    /// </summary>
    /// <remarks>
    /// The items of the collection are <see cref="InvoiceInfo"/> objects.
    /// </remarks>
    [Serializable]
#if WINFORMS
    public partial class InvoiceList : ReadOnlyBindingListBase<InvoiceList, InvoiceInfo>
#else
    public partial class InvoiceList : ReadOnlyListBase<InvoiceList, InvoiceInfo>
#endif
    {

        #region Collection Business Methods

        /// <summary>
        /// Determines whether a <see cref="InvoiceInfo"/> item is in the collection.
        /// </summary>
        /// <param name="invoiceId">The InvoiceId of the item to search for.</param>
        /// <returns><c>true</c> if the InvoiceInfo is a collection item; otherwise, <c>false</c>.</returns>
        public bool Contains(Guid invoiceId)
        {
            foreach (var invoiceInfo in this)
            {
                if (invoiceInfo.InvoiceId == invoiceId)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Find Methods

        /// <summary>
        /// Finds a <see cref="InvoiceInfo"/> item of the <see cref="InvoiceList"/> collection, based on a given InvoiceId.
        /// </summary>
        /// <param name="invoiceId">The InvoiceId.</param>
        /// <returns>A <see cref="InvoiceInfo"/> object.</returns>
        public InvoiceInfo FindInvoiceInfoByInvoiceId(Guid invoiceId)
        {
            for (var i = 0; i < this.Count; i++)
            {
                if (this[i].InvoiceId.Equals(invoiceId))
                {
                    return this[i];
                }
            }

            return null;
        }

        #endregion

        #region Factory Methods

        /// <summary>
        /// Factory method. Loads a <see cref="InvoiceList"/> collection.
        /// </summary>
        /// <returns>A reference to the fetched <see cref="InvoiceList"/> collection.</returns>
        public static InvoiceList GetInvoiceList()
        {
            return DataPortal.Fetch<InvoiceList>();
        }

        /// <summary>
        /// Factory method. Asynchronously loads a <see cref="InvoiceList"/> collection.
        /// </summary>
        /// <param name="callback">The completion callback method.</param>
        public static void GetInvoiceList(EventHandler<DataPortalResult<InvoiceList>> callback)
        {
            DataPortal.BeginFetch<InvoiceList>(callback);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceList"/> class.
        /// </summary>
        /// <remarks> Do not use to create a Csla object. Use factory methods instead.</remarks>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public InvoiceList()
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
        /// Loads a <see cref="InvoiceList"/> collection from the database.
        /// </summary>
        protected void DataPortal_Fetch()
        {
            var args = new DataPortalHookArgs();
            OnFetchPre(args);
            using (var dalManager = DalFactoryInvoices.GetManager())
            {
                var dal = dalManager.GetProvider<IInvoiceListDal>();
                var data = dal.Fetch();
                Fetch(data);
            }
            OnFetchPost(args);
        }

        /// <summary>
        /// Loads all <see cref="InvoiceList"/> collection items from the given list of InvoiceInfoDto.
        /// </summary>
        /// <param name="data">The list of <see cref="InvoiceInfoDto"/>.</param>
        private void Fetch(List<InvoiceInfoDto> data)
        {
            IsReadOnly = false;
            var rlce = RaiseListChangedEvents;
            RaiseListChangedEvents = false;
            foreach (var dto in data)
            {
                Add(DataPortal.FetchChild<InvoiceInfo>(dto));
            }
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
