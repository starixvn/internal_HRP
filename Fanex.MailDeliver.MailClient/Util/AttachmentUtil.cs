namespace Fanex.MailDeliver.MailClient.Util
{
    using System.IO;
    using Fanex.MailDeliver.Model;

    public class AttachmentUtil
    {
        public static void Format(ref MailRecord record)
        {
            if (record.Attachments != null)
            {
                foreach (var attachment in record.Attachments)
                {
                    if (attachment.Content == null && !string.IsNullOrEmpty(attachment.FileName))
                    {
                        var stream = System.IO.File.Open(attachment.FileName, FileMode.Open);
                        var buffer = new byte[stream.Length];
                        stream.Read(buffer, 0, buffer.Length);
                        stream.Flush();
                        stream.Close();
                        attachment.Content = new MemoryStream(buffer);
                    }

                    attachment.FileName = Path.GetFileName(attachment.FileName);
                }
            }

            if (record.AlternateView != null)
            {
                foreach (var resource in record.AlternateView)
                {
                    if (resource.StreamContent == null && !string.IsNullOrEmpty(resource.FileName))
                    {
                        var stream = System.IO.File.Open(resource.FileName, FileMode.Open);
                        var buffer = new byte[stream.Length];
                        stream.Read(buffer, 0, buffer.Length);
                        stream.Flush();
                        stream.Close();
                        resource.StreamContent = new MemoryStream(buffer);
                    }

                    resource.FileName = Path.GetFileName(resource.FileName);
                }
            }
        }

        public static void LinkedResource(ref MailRecord record)
        {
            foreach (var resource in record.AlternateView)
            {
                if (resource.StreamContent == null)
                {
                    var stream = System.IO.File.Open(resource.FileName, FileMode.Open);
                    var buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                    stream.Flush();
                    resource.StreamContent = new MemoryStream(buffer);
                }

                resource.FileName = Path.GetFileName(resource.FileName);
            }
        }
    }
}