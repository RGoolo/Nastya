using System.IO;

namespace Model.Files.FileTokens
{

	public interface IChatFileWorker
	{
		FileStream ReadStream(IFileToken token);
		FileStream WriteStream(IFileToken token);

		string ReadToEnd(IFileToken token);
		void SaveObject<T>(T instance, IFileToken token);
	}
}