using System.Globalization;

namespace Givt.Platform.Pdf.Interfaces;

public interface IPdfService
{
   Task<IFileData> CreateSinglePaymentReport(object report, CultureInfo cultureInfo, CancellationToken cancellationToken);
}
