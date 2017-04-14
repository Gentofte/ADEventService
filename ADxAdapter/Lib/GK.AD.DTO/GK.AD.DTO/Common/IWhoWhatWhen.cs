using System;

namespace GK.AD.DTO
{
    // ================================================================================
    public interface IWhoWhatWhen
    {
        string Who { get; }
        string What { get; }
        DateTime When { get; }
    }
}
