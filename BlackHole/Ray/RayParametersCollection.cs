using System;
using System.Collections;
using System.ComponentModel;
using System.Data.Common;

namespace BlackHole.Ray
{

    [
    Editor("Microsoft.VSDesigner.Data.Design.DBParametersEditor, " + AssemblyRef.MicrosoftVSDesigner, "System.Drawing.Design.UITypeEditor, " + AssemblyRef.SystemDrawing),
    ListBindable(false)
    ]
    public sealed partial class RayParameterCollection : DbParameterCollection
    {
        private bool _rebindCollection;   // The collection needs to be (re)bound

        private static Type ItemType = typeof(RayParameter);

        internal RayParameterCollection() : base()
        {
        }

        internal bool RebindCollection
        {
            get { return _rebindCollection; }
            set { _rebindCollection = value; }
        }

        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        new public RayParameter this[int index]
        {
            get
            {
                return (RayParameter)GetParameter(index);
            }
            set
            {
                SetParameter(index, value);
            }
        }

        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        new public RayParameter this[string parameterName]
        {
            get
            {
                return (RayParameter)GetParameter(parameterName);
            }
            set
            {
                SetParameter(parameterName, value);
            }
        }

        public RayParameter Add(RayParameter value)
        {
            // MDAC 59206
            Add((object)value);
            return value;
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        [ObsoleteAttribute("Add(String parameterName, Object value) has been deprecated.  Use AddWithValue(String parameterName, Object value).  http://go.microsoft.com/fwlink/?linkid=14202", false)] // 79027
        public RayParameter Add(string parameterName, object value)
        {
            // MDAC 59206
            return Add(new RayParameter(parameterName, value));
        }

        public RayParameter AddWithValue(string parameterName, object value)
        {
            // MDAC 79027
            return Add(new RayParameter(parameterName, value));
        }

        public RayParameter Add(string parameterName, RayType RayType)
        {
            return Add(new RayParameter(parameterName, RayType));
        }

        public RayParameter Add(string parameterName, RayType RayType, int size)
        {
            return Add(new RayParameter(parameterName, RayType, size));
        }

        public RayParameter Add(string parameterName, RayType RayType, int size, string sourceColumn)
        {
            return Add(new RayParameter(parameterName, RayType, size, sourceColumn));
        }

        public void AddRange(RayParameter[] values)
        {
            // V1.2.3300
            AddRange((Array)values);
        }

        // Walks through the collection and binds each parameter
        //
        internal void Bind(RayCommand command, CMDWrapper cmdWrapper, CNativeBuffer parameterBuffer)
        {
            for (int i = 0; i < Count; ++i)
            {
                this[i].Bind(cmdWrapper.StatementHandle, command, checked((short)(i + 1)), parameterBuffer, true);
            }
            _rebindCollection = false;
        }

        internal int CalcParameterBufferSize(RayCommand command)
        {
            // Calculate the size of the buffer we need
            int parameterBufferSize = 0;

            for (int i = 0; i < Count; ++i)
            {
                if (_rebindCollection)
                {
                    this[i].HasChanged = true;
                }
                this[i].PrepareForBind(command, (short)(i + 1), ref parameterBufferSize);

                parameterBufferSize = (parameterBufferSize + (IntPtr.Size - 1)) & ~(IntPtr.Size - 1);          // align buffer;
            }
            return parameterBufferSize;
        }

        // Walks through the collection and clears the parameters
        //
        internal void ClearBindings()
        {
            for (int i = 0; i < Count; ++i)
            {
                this[i].ClearBinding();
            }
        }

        override public bool Contains(string value)
        { // WebData 97349
            return (-1 != IndexOf(value));
        }

        public bool Contains(RayParameter value)
        {
            return (-1 != IndexOf(value));
        }

        public void CopyTo(RayParameter[] array, int index)
        {
            CopyTo((Array)array, index);
        }

        private void OnChange()
        {
            _rebindCollection = true;
        }

        internal void GetOutputValues(CMDWrapper cmdWrapper)
        {
            // mdac 88542 - we will not read out the parameters if the collection has changed
            if (!_rebindCollection)
            {
                CNativeBuffer parameterBuffer = cmdWrapper._nativeParameterBuffer;
                for (int i = 0; i < Count; ++i)
                {
                    this[i].GetOutputValue(parameterBuffer);
                }
            }
        }

        public int IndexOf(RayParameter value)
        {
            return IndexOf((object)value);
        }

        public void Insert(int index, RayParameter value)
        {
            Insert(index, (object)value);
        }

        public void Remove(RayParameter value)
        {
            Remove((object)value);
        }
    }
}
