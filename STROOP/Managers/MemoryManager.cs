﻿using STROOP.Controls;
using STROOP.Structs;
using STROOP.Structs.Configurations;
using STROOP.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STROOP.Managers
{
    public class MemoryManager
    {
        private BetterTextbox _textBoxMemoryStartAddress;
        private Button _buttonMemoryButtonGo;
        private CheckBox _checkBoxMemoryUpdateContinuously;
        private RichTextBox _richTextBoxMemory;

        public uint? Address { get; private set; }

        public MemoryManager(TabPage tabControl)
        {
            _textBoxMemoryStartAddress = tabControl.Controls["textBoxMemoryStartAddress"] as BetterTextbox;
            _buttonMemoryButtonGo = tabControl.Controls["buttonMemoryButtonGo"] as Button;
            _checkBoxMemoryUpdateContinuously = tabControl.Controls["checkBoxMemoryUpdateContinuously"] as CheckBox;
            _richTextBoxMemory = tabControl.Controls["richTextBoxMemory"] as RichTextBox;

            _textBoxMemoryStartAddress.AddEnterAction(() => TryToSetAddressAndUpdateMemory());
            _buttonMemoryButtonGo.Click += (sender, e) => TryToSetAddressAndUpdateMemory();

            Address = null;
        }

        private void TryToSetAddressAndUpdateMemory()
        {
            uint? addressNullable = ParsingUtilities.ParseHexNullable(_textBoxMemoryStartAddress.Text);
            if (addressNullable.HasValue) SetAddressAndUpdateMemory(addressNullable.Value);
        }

        public void SetAddressAndUpdateMemory(uint address)
        {
            _textBoxMemoryStartAddress.Text = HexUtilities.Format(address, 8);
            Address = address;
            UpdateMemory();
        }

        private void UpdateMemory()
        {
            if (!Address.HasValue) return;
            byte[] bytes = Config.Stream.ReadRam(Address.Value, (int)ObjectConfig.StructSize);
            _richTextBoxMemory.Text = FormatBytesForHexEditorDisplay(bytes);
        }

        private string FormatBytesForHexEditorDisplay(byte[] bytes)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(HexUtilities.Format(bytes[i], 2, false));
                builder.Append(i % 16 == 15 ? "\r\n" : " ");
            }
            return builder.ToString();
        }

        public void Update(bool updateView)
        {
            if (!updateView) return;

            if (_checkBoxMemoryUpdateContinuously.Checked) UpdateMemory();
        }
    }
}
