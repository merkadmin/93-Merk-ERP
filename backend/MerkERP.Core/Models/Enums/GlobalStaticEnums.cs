using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerkERP.Core.Models.Enums
{
	public enum BarcodeTypeEnum
	{
		None       = 0,
		EAN13      = 1,
		EAN8       = 2,
		UPCA       = 3,
		UPCE       = 4,
		Code39     = 5,
		Code128    = 6,
		ITF14      = 7,
		GS1128     = 8,
		QRCode     = 9,
		DataMatrix = 10,
		Custom     = 11,
	}
}
