#pragma once

#include <iomanip>
#include <sstream>
#include <stdexcept>
#include <string>
#include <Windows.h>

class FileStream
{
public:
	static const DWORD kDefaultBufferSize = 4096;
public:
	enum class Mode
	{
		Open,
		OpenWithoutBuffering,
		Create,
		Truncate,
		Append
	};
	FileStream(std::wstring filepath, Mode mode) : FileStream(filepath, mode, kDefaultBufferSize)
	{
	}
	FileStream(std::wstring filepath, Mode mode, const DWORD buffersize)
		: filepath_(filepath), mode_(mode), buffersize_(buffersize)
	{
		AllocateBuffer();
		OpenFile();
	}
	//FileStream(const FileStream& that) = delete;
	//FileStream(FileStream&& other)
	//	: filepath_(std::move(other.filepath_)), mode_(other.mode_), buffersize_(other.buffersize_)
	//{
	//	*this = std::move(other);
	//}
	//FileStream& operator=(FileStream&& other)
	//{
	//	if (this != &other)
	//	{
	//		if (other.filepath_.size() > 0)
	//		{
	//			filepath_ = std::move(other.filepath_);
	//		}

	//		mode_ = other.mode_;
	//		buffersize_ = other.buffersize_;
	//		readindex_ = other.readindex_;
	//		readlength_ = other.readlength_;
	//		writeindex_ = other.writeindex_;
	//		other.writeindex_ = 0;
	//		buffer_ = other.buffer_;
	//		other.buffer_ = NULL;
	//		filehandle_ = other.filehandle_;
	//		other.filehandle_ = INVALID_HANDLE_VALUE;
	//		lasterror_ = other.lasterror_;
	//	}

	//	return *this;
	//}
	virtual ~FileStream()
	{
		Close();
		FreeBuffer();
	}
	DWORD Read(PBYTE buffer, DWORD count);
	void Write(PBYTE buffer, DWORD count);
	void Flush();
	void Close()
	{
		Flush();
		CloseFile();
	}
	DWORD lasterror() const { return lasterror_; }
private:
	void AllocateBuffer();
	void OpenFile();
	DWORD Read(PBYTE buffer, DWORD offset, DWORD count);
	DWORD Write(PBYTE buffer, DWORD offset, DWORD count);
	void FlushWrite();
	void CloseFile();
	void FreeBuffer();
	DWORD readindex_{};
	DWORD readlength_{};
	DWORD writeindex_{};
	PBYTE buffer_{ NULL };
	std::wstring filepath_;
	Mode mode_;
	DWORD buffersize_;
	HANDLE filehandle_{ NULL };
	DWORD lasterror_{};
};

inline void FileStream::AllocateBuffer()
{
	buffer_ = (PBYTE)VirtualAlloc(NULL, buffersize_, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
}

inline void FileStream::FreeBuffer()
{
	if (buffer_ != NULL)
	{
		VirtualFree(buffer_, 0, MEM_RELEASE);
	}
}