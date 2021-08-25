using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Midi;
using Midi.Devices;
using Midi.Messages;

namespace MidiCustomize.Midi
{
    public class Midi
    {
        #region Properties
        /// <summary>
        /// Input midi from midi port
        /// </summary>
        public IInputDevice Input { get; set; }

        /// <summary>
        /// Output midi from virtual midi using LoopMIDI
        /// </summary>
        public IOutputDevice Output { get; set; }

        /// <summary>
        /// Logic to convert notes
        /// </summary>
        public IConvertLogic Logic { get; set; }

        public bool IsRunning { get; private set; }
        #endregion

        #region Events
        public event EventHandler Sent;
        #endregion

        #region Constructors
        /// <summary>
        /// Declare new Midi object
        /// </summary>
        public Midi() { }

        /// <summary>
        /// Declare new Midi object
        /// </summary>
        /// <param name="input">input object like midi input</param>
        /// <param name="output">Virtual midi object using LoopMIDI</param>
        /// <param name="logic">Logic to convert notes</param>
        public Midi(IInputDevice input, IOutputDevice output, IConvertLogic logic)
        {
            Input = input;
            Output = output;
            Logic = logic;
        }
        #endregion

        #region Methods
        public void StartReceiving(Clock clock)
        {
            if (IsRunning)
                return;

            Input.Open();
            Output.Open();

            Input.StartReceiving(clock);
            //Output.SilenceAllNotes();

            IsRunning = true;

            Input.PitchBend += Input_PitchBend;
            Input.NoteOn += Input_NoteOn;
            Input.NoteOff += Input_NoteOff;
            Input.ControlChange += Input_ControlChange;
            Input.ProgramChange += Input_ProgramChange;
        }

        private void Input_ProgramChange(ProgramChangeMessage msg)
        {
            var m = Logic.Convert(msg);

            Output.SendProgramChange(m.Channel, m.Instrument);
            Sent?.Invoke(m, EventArgs.Empty);
        }

        private void Input_ControlChange(ControlChangeMessage msg)
        {
            var m = Logic.Convert(msg);

            Output.SendControlChange(m.Channel, m.Control, m.Value);
            Sent?.Invoke(m, EventArgs.Empty);
        }

        private void Input_NoteOff(NoteOffMessage msg)
        {
            var m = Logic.Convert(msg);

            Output.SendNoteOff(m.Channel, m.Pitch, m.Velocity);
            Sent?.Invoke(m, EventArgs.Empty);
        }

        private void Input_NoteOn(NoteOnMessage msg)
        {
            if (msg.Velocity == 0)
                return;

            var m = Logic.Convert(msg);

            Output.SendNoteOn(m.Channel, m.Pitch, m.Velocity);
            Sent?.Invoke(m, EventArgs.Empty);
        }

        private void Input_PitchBend(PitchBendMessage msg)
        {
            var m = Logic.Convert(msg);
            
            Output.SendPitchBend(m.Channel, m.Value);
            Sent?.Invoke(m, EventArgs.Empty);
        }

        public void CloseReceiving()
        {
            if (!IsRunning)
                return;

            Input.StopReceiving();
            Input.RemoveAllEventHandlers();
            //Output.SilenceAllNotes();

            Input.Close();
            Output.Close();

            IsRunning = false;
        }
        #region NonStatic

        #endregion

        #region Static
        /// <summary>
        /// Get devices. If T == IDeviceBase, it return null
        /// </summary>
        /// <typeparam name="T">IInputDevice or IOutputDevice</typeparam>
        /// <returns>Device list</returns>
        public static IEnumerable<T> GetDevices<T>() where T : IDeviceBase
        {
            DeviceManager.UpdateInputDevices();
            DeviceManager.UpdateOutputDevices();

            if (typeof(T) == typeof(IInputDevice))
                return (IEnumerable<T>)DeviceManager.InputDevices;

            else if (typeof(T) == typeof(IOutputDevice))
                return (IEnumerable<T>)DeviceManager.OutputDevices;

            else
                return null;
        }
        #endregion

        #endregion
    }
}
