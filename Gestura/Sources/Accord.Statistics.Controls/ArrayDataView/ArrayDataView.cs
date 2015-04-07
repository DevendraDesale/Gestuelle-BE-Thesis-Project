#region LIbrary Files
using System;
using System.ComponentModel;
using System.Globalization;
#endregion

namespace Accord.Controls
{

    /// <summary>
    /// Represents a data bondable, customized view of two dimensional array
    /// </summary>
    public class ArrayDataView : IBindingList
    {
        #region Variables

        /// <summary>
        /// array rows.
        /// </summary>
        private ArrayRowView[] rows;

        /// <summary>
        /// Data which will be binded.
        /// </summary>
        private Array data;

        /// <summary>
        /// Alternative column names.
        /// </summary>
        private string[] columnNames;
        #endregion // Variables

        #region Events
        /// <summary>
        ///   Raised when the list changes.
        /// </summary>
        public event System.ComponentModel.ListChangedEventHandler ListChanged;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new ArrayDataView from array.
        /// </summary>
        /// <param name="array">array of data.</param>
        public ArrayDataView(Array array)
        {
            if (array.Rank > 2)
                throw new ArgumentException("Supports only up to two dimentional arrays", "array");

            this.data = array;

            if (array.Rank == 2)
            {
                rows = new ArrayRowView[array.GetLength(0)];

                for (int i = 0; i < rows.Length; i++)
                {
                    rows[i] = new ArrayRowView(this, i);
                }
            }
            else
            {
                rows = new ArrayRowView[] { new ArrayRowView(this, 0) };
            }
        }

        /// <summary>
        /// Initializes a new ArrayDataView from array with custom column names.
        /// </summary>
        /// <param name="array">array of data.</param>
        /// <param name="columnNames">collection of column names.</param>
        public ArrayDataView(Array array, object[] columnNames)
            : this(array)
        {
            if (array.Rank == 2)
            {
                if (columnNames.Length != array.GetLength(1))
                    throw new ArgumentException("column names must correspond to array columns.", "columnNames");
            }
            else
            {
                if (columnNames.Length != array.GetLength(0))
                    throw new ArgumentException("column names must correspond to array columns.", "columnNames");
            }

            this.columnNames = new string[columnNames.Length];
            for (int i = 0; i < columnNames.Length; i++)
            {
                this.columnNames[i] = columnNames[i].ToString();
            }
        }

        #endregion // Constructors

        #region Properties

        internal string[] ColumnNames
        {
            get
            {
                if (columnNames == null)
                {
                    if (Data.Rank == 2)
                    {
                        columnNames = new string[Data.GetLength(1)];
                    }
                    else
                    {
                        columnNames = new string[Data.GetLength(0)];
                    }

                    for (int i = 0; i < columnNames.Length; i++)
                    {
                        columnNames[i] = i.ToString(CultureInfo.CurrentCulture);
                    }
                }
                return columnNames;
            }
        }

        /// <summary>
        ///   Gets or sets the array currently bound.
        /// </summary>
        public Array Data
        {
            get { return data; }
           // set { data = value; }
        }
        #endregion // Properties

        #region Methods
        /// <summary>
        ///   Resets the data binding.
        /// </summary>
        public void Reset()
        {
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        #endregion

        #region IBindingList Members
        /// <summary>
        ///   Does nothing.
        /// </summary>
        public void AddIndex(PropertyDescriptor property)
        {
        }

        /// <summary>
        ///   Arrays do not allow for member insertion.
        /// </summary>
        public bool AllowNew
        {
            get { return false; }
        }

        /// <summary>
        ///   Does nothing.
        /// </summary>
        public void ApplySort(PropertyDescriptor property, System.ComponentModel.ListSortDirection direction)
        {
        }

        /// <summary>
        ///   Does nothing.
        /// </summary>
        public PropertyDescriptor SortProperty
        {
            get { return null; }
        }

        /// <summary>
        ///   Does nothing.
        /// </summary>
        public int Find(PropertyDescriptor property, object key)
        {
            return 0;
        }

        /// <summary>
        ///   Does nothing.
        /// </summary>
        public bool SupportsSorting
        {
            get { return false; }
        }

        /// <summary>
        ///   Does nothing.
        /// </summary>
        public bool IsSorted
        {
            get { return false; }
        }

        /// <summary>
        ///   Arrays do not allow member removal.
        /// </summary>
        public bool AllowRemove
        {
            get { return false; }
        }

        /// <summary>
        ///   Does nothing.
        /// </summary>
        public bool SupportsSearching
        {
            get { return false; }
        }

        /// <summary>
        ///   Does nothing.
        /// </summary>
        public ListSortDirection SortDirection
        {
            get { return new System.ComponentModel.ListSortDirection(); }
        }



        private void OnListChanged(ListChangedEventArgs e)
        {
            if (ListChanged != null)
            {
                ListChanged(this, e);
            }
        }

        /// <summary>
        ///   Does nothing.
        /// </summary>
        public bool SupportsChangeNotification
        {
            get { return true; }
        }

        /// <summary>
        ///   Does nothing.
        /// </summary>
        public void RemoveSort()
        {
        }

        /// <summary>
        ///   Does nothing.
        /// </summary>
        public object AddNew()
        {
            return null;
        }

        /// <summary>
        ///   Gets whether this view allows editing. Always true.
        /// </summary>
        public bool AllowEdit
        {
            get { return true; }
        }

        /// <summary>
        ///   Does nothing.
        /// </summary>
        public void RemoveIndex(PropertyDescriptor property)
        {
        }

        #endregion

        #region IList Members

        /// <summary>
        ///   This view is read only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        ///   Gets a row from this view.
        /// </summary>
        public object this[int index]
        {
            get { return rows[index]; }
            set { }
        }
        
        public void RemoveAt(int index)
        {
        }

        public void Insert(int index, object value)
        {
        }

        public void Remove(object value)
        {
        }

        public bool Contains(object value)
        {
            return false;
        }

        public void Clear()
        {
        }

        public int IndexOf(object value)
        {
            return 0;
        }

        public int Add(object value)
        {
            return 0;
        }

        public bool IsFixedSize
        {
            get { return true; }
        }

        #endregion

        #region ICollection Members

        /// <summary>
        ///   Returns false.
        /// </summary>
        public bool IsSynchronized
        {
            get { return false; }
        }

        /// <summary>
        ///   Gets the length of the array.
        /// </summary>
        public int Count
        {
            get { return rows.Length; }
        }

        /// <summary>
        ///   Multidimensional arrays do not support Array copying.
        /// </summary>
        public void CopyTo(System.Array array, int index)
        {
        }

        public object SyncRoot
        {
            get { return null; }
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        ///   Gets the array enumerator.
        /// </summary>
        public System.Collections.IEnumerator GetEnumerator()
        {
            return rows.GetEnumerator();
        }

        #endregion
    }

}
