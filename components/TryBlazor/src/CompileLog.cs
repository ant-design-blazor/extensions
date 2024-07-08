using Microsoft.Extensions.Logging;

namespace AntDesign.Extensions;

public record CompileLog(string Content = "", LogLevel LogLevel = LogLevel.Debug);