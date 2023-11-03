using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace AntDesign.Extensions;

public enum CodeType
{
    Razor = 0,

    CSharp = 1,
}

public class CodeFile
{
    public CodeFile():this("","")
    {
        
    }
    public CodeFile(string code, string fileName)
    {
        this.Code = code;
        this.FileName = fileName;
    }

    public string Code { get; set; }

    public string FileName { get; set; }

    private CodeType? _codeType;

    public CodeType CodeType
    {
        get
        {
            if (!_codeType.HasValue)
            {
                var extension = Path.GetExtension(FileName);

                _codeType = extension switch
                {
                    ".razor" => CodeType.Razor,
                    ".cs" => CodeType.CSharp,
                    _ => throw new NotSupportedException($"Unsupported extension: {extension}"),
                };
            }

            return _codeType.Value;
        }
    }
}
