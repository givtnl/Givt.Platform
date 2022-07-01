﻿using MediatR;
using Microsoft.Extensions.Primitives;

namespace Givt.Platform.Payments.Interfaces;

public interface IRawSinglePaymentNotification : INotification
{
    string RawData { get; set; }
    IDictionary<string, StringValues> MetaData { get; set; }
}
