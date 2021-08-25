using Midi.Enums;
using Midi.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidiCustomize.Midi
{
    // TODO : Make logic with mecro file.
    public class PianoConverterLogic : ConvertLogic, IConvertLogic
    {
        public PitchBendMessage Convert(PitchBendMessage note) => note;

        public NoteOnMessage Convert(NoteOnMessage note)
            => new NoteOnMessage(note.Device, note.Channel, ConvertPitch(note.Pitch), note.Velocity, note.Time);

        public NoteOffMessage Convert(NoteOffMessage note)
            => new NoteOffMessage(note.Device, note.Channel, ConvertPitch(note.Pitch), note.Velocity, note.Time);

        public ControlChangeMessage Convert(ControlChangeMessage note) => note;

        public ProgramChangeMessage Convert(ProgramChangeMessage note) => note;

        private Pitch GetPitch(Pitch p, Pitch pBase, int i)
        {
            var r = Math.DivRem(p - pBase, Keys.Length, out int l);
            return Enum.Parse<Pitch>(Keys[l] + (r + i));
        }

        private Pitch ConvertPitch(Pitch p)
        {
            if (Pitch.BNeg1 <= p && p <= Pitch.FSharp0)
                return GetPitch(p, Pitch.BNeg1, 3);

            else if (Pitch.A0 <= p && p <= Pitch.E1)
            {
                return p switch
                {
                    Pitch.ASharp0 => Pitch.CSharp3,
                    Pitch.B0 => Pitch.DSharp3,
                    Pitch.CSharp1 => Pitch.FSharp3,
                    Pitch.D1 => Pitch.GSharp3,
                    Pitch.DSharp1 => Pitch.ASharp3,
                    _ => Pitch.CNeg1
                };
            }

            else if (Pitch.G1 <= p && p <= Pitch.D2)
                return GetPitch(p, Pitch.G1, 4);

            else if (Pitch.F2 <= p && p <= Pitch.C3)
            {
                return p switch
                {
                    Pitch.FSharp2 => Pitch.CSharp4,
                    Pitch.G2 => Pitch.DSharp4,
                    Pitch.A2 => Pitch.FSharp4,
                    Pitch.ASharp2 => Pitch.GSharp4,
                    Pitch.B2 => Pitch.ASharp4,
                    _ => Pitch.CNeg1
                };
            }

            else if (Pitch.DSharp3 <= p && p <= Pitch.ASharp3)
                return GetPitch(p, Pitch.DSharp3, 5);

            else if (Pitch.CSharp4 <= p && p <= Pitch.GSharp4)
            {
                return p switch
                {
                    Pitch.D4 => Pitch.CSharp5,
                    Pitch.DSharp4 => Pitch.DSharp5,
                    Pitch.F4 => Pitch.FSharp5,
                    Pitch.FSharp4 => Pitch.GSharp5,
                    Pitch.G4 => Pitch.ASharp5,
                    _ => Pitch.CNeg1
                };
            }

            else if (Pitch.B4 <= p && p <= Pitch.FSharp5)
                return GetPitch(p, Pitch.B4, 6);

            else if (Pitch.A5 <= p && p <= Pitch.E6)
            {
                return p switch
                {
                    Pitch.ASharp5 => Pitch.CSharp6,
                    Pitch.B5 => Pitch.DSharp6,
                    Pitch.CSharp6 => Pitch.FSharp6,
                    Pitch.D6 => Pitch.GSharp6,
                    Pitch.DSharp6 => Pitch.ASharp6,
                    _ => Pitch.CNeg1
                };
            }

            else
                return Pitch.CNeg1;
        }
    }
}
