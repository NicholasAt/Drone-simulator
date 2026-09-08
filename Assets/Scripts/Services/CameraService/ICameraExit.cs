using Cysharp.Threading.Tasks;
using System.Threading;

namespace Assets.Scripts.Services.CameraService
{
    public interface ICameraEnterParam3<T1, T2, T3> : ICameraExit
    {
        UniTask Enter(T1 t1, T2 t2, T3 t3);
    }

    public interface ICameraEnter : ICameraExit
    {
        UniTask Enter();
    }

    public interface ICameraExit
    {
        UniTask Prepare();
        UniTask Exit();
    }
}