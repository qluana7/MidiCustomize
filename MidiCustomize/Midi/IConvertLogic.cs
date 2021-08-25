using System;
using System.Collections.Generic;
using System.Text;
using Midi.Messages;

namespace MidiCustomize.Midi
{
    public interface IConvertLogic
    {
        public PitchBendMessage Convert(PitchBendMessage note);

        public NoteOnMessage Convert(NoteOnMessage note);

        public NoteOffMessage Convert(NoteOffMessage note);

        public ControlChangeMessage Convert(ControlChangeMessage note);

        public ProgramChangeMessage Convert(ProgramChangeMessage note);
    }
}
