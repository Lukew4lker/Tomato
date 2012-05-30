﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Tomato.Hardware
{
    public class GenericClock : Device
    {
        public ushort InterruptMessage { get; set; }
        public ushort Frequency { get; set; }
        public Timer Clock { get; set; }
        public ushort ElapsedTicks { get; set; }

        public override uint DeviceID
        {
            get { return 0x12d0b402; }
        }

        public override uint ManufacturerID
        {
            get { return 0x0; }
        }

        public override ushort Version
        {
            get { return 1; }
        }

        public override string FriendlyName
        {
            get { return "Generic Clock (compatible)"; }
        }

        public override int HandleInterrupt()
        {
            switch (AttachedCPU.A)
            {
                case 0:
                    Frequency = AttachedCPU.B;
                    if (Frequency != 0)
                        Clock = new Timer(Tick, null, (int)(1000 / (60d / Frequency)), Timeout.Infinite);
                    else
                    {
                        if (Clock != null)
                        {
                            Clock.Dispose();
                            Clock = null;
                        }
                    }
                    break;
                case 1:
                    AttachedCPU.C = ElapsedTicks;
                    break;
                case 2:
                    InterruptMessage = AttachedCPU.B;
                    break;
            }
            ElapsedTicks = 0;
            return 0;
        }

        public void Tick(object o)
        {
            try
            {
                if (!AttachedCPU.IsRunning)
                {
                    Clock = new Timer(Tick, null, (int)(1000 / (60d / Frequency)), Timeout.Infinite);
                    return;
                }
                if (InterruptMessage != 0)
                    AttachedCPU.FireInterrupt(InterruptMessage);
                ElapsedTicks++;
                Clock = new Timer(Tick, null, (int)(1000 / (60d / Frequency)), Timeout.Infinite);
            }
            catch { }
        }

        public override void Reset()
        {
            throw new NotImplementedException();
        }
    }
}