/// <summary>
/// Provides few methods for Stream class.
/// </summary>
public static class StreamHelpers
{
    /// <summary>
    /// Reads a specific amount of bytes from the current stream 
    /// and writes them to another stream.
    /// </summary>
    /// <param name="outStream">The stream to which the contents of the current stream will be copied.</param>
    /// <param name="size">The amount of bytes to read.</param>
    /// <param name="showProgress">Show the amount of data read during the process in percentage.</param>
    public static void CopyStreamTo(this Stream inStream, Stream outStream, long size, bool showProgress)
    {
        int bufferSize = 81920;
        long amountRemaining = size;
        var copyArray = new byte[bufferSize];
        long amountCopied = 0;
        decimal currentAmount;

        while (amountRemaining > 0)
        {
            long readAmount = Math.Min(bufferSize, amountRemaining);

            _ = inStream.Read(copyArray, 0, (int)readAmount);
            outStream.Write(copyArray, 0, (int)readAmount);

            amountRemaining -= readAmount;

            amountCopied += readAmount;

            if (showProgress)
            {
                currentAmount = Math.Round(((decimal)amountCopied / size) * 100);
                Console.Write("\r{0}", "Copied " + currentAmount + "%");
            }
        }
    }


    /// <summary>
    /// Pads null bytes into a stream.
    /// </summary>
    /// <param name="padAmount">The amount of bytes to pad.</param>
    public static void PadNull(this Stream stream, long padAmount)
    {
        for (long p = 0; p < padAmount; p++)
        {
            stream.WriteByte(0);
        }
    }
}