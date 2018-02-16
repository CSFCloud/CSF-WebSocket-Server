using CSFCloud.WebSocket.Util;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CSFCloud.WebSocket.Loopers {
    internal class Looper {

        private int initialWait;
        private int loopWait;

        private Func<Task> loopFunc = null;
        private Action loopAct = null;

        protected Thread thread;
        bool keepRunning = true;

        public Looper(int iw, int lw) {
            this.initialWait = iw;
            this.loopWait = lw;
        }

        public void SetLoopFunction(Func<Task> act) {
            loopFunc = act;
        }

        public void SetLoopAction(Action act) {
            loopAct = act;
        }

        public void Start() {
            if (loopAct == null && loopFunc == null) {
                throw new Exception("Loop not set");
            }
            keepRunning = true;

            thread = new Thread(async () => {
                if (this.initialWait > 0) {
                    Thread.Sleep(this.initialWait);
                }
                while (keepRunning) {
                    try {
                        if (loopFunc != null) {
                            await loopFunc.Invoke();
                        } else if (loopAct != null) {
                            loopAct.Invoke();
                        }
                    } catch (Exception e) {
                        Logger.Error($"Loop exception: {e.Message}");
                    }
                    if (this.loopWait > 0) {
                        Thread.Sleep(this.loopWait);
                    }
                }
            });
            thread.Start();
        }

        public void Stop() {
            keepRunning = false;
        }

    }
}
