﻿using BlackHole.Identifiers;

namespace BlackHole.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="Dto"></typeparam>
    /// <typeparam name="Tsource"></typeparam>
    /// <typeparam name="TOther"></typeparam>
    public interface IPrejoin<Dto, Tsource, TOther> where Dto : BHDtoIdentifier
    {

    }
}
