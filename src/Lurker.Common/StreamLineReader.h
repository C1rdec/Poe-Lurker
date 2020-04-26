#pragma once

#include "FileStream.h"

#include <Windows.h>

#include <string>

class StreamLineReader
{
private:
	static const DWORD kDefaultBufferSize = 32768;
public:
	enum class Encoding
	{
		UTF8
	};
	StreamLineReader(std::wstring filepath) : StreamLineReader(filepath, Encoding::UTF8)
	{
	}
	StreamLineReader(std::wstring filepath, Encoding encoding)
		: filestream_(FileStream(filepath, FileStream::Mode::Open)), encoding_(encoding), buffersize_(kDefaultBufferSize)
	{
		AllocateBuffer();
	}
	StreamLineReader(FileStream&& filestream) : StreamLineReader(std::forward<FileStream>(filestream), Encoding::UTF8)
	{
	}
	StreamLineReader(FileStream&& filestream, Encoding encoding) : filestream_(std::move(filestream)), encoding_(encoding), buffersize_(kDefaultBufferSize)
	{
		AllocateBuffer();
	}
	~StreamLineReader()
	{
		FreeBuffer();
	}
	std::wstring ReadLine();
	BOOL EndOfStream()
	{
		return ReadBytes() == 0;
	}
	void Close()
	{
		filestream_.Close();
	}
private:
	void AllocateBuffer();
	void FreeBuffer();
	DWORD ReadBytes();
	FileStream filestream_;
	Encoding encoding_;
	PBYTE buffer_;
	DWORD buffersize_;
	DWORD readindex_{};
	DWORD readlength_{};
};

inline void StreamLineReader::AllocateBuffer()
{
	buffer_ = (PBYTE)VirtualAlloc(NULL, buffersize_, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
}

inline void StreamLineReader::FreeBuffer()
{
	if (buffer_ != NULL)
	{
		VirtualFree(buffer_, 0, MEM_RELEASE);
	}
}

inline DWORD StreamLineReader::ReadBytes()
{
	if (readlength_ - readindex_ == 0)
	{
		readindex_ = 0;
		readlength_ = filestream_.Read(buffer_, buffersize_);
	}

	return readlength_;
}

inline std::wstring StreamLineReader::ReadLine()
{
	BOOL eol = FALSE;
	std::string raw;
	while (!eol)
	{
		DWORD bufferbytes = readlength_ - readindex_;
		if (bufferbytes == 0)
		{
			bufferbytes = ReadBytes();
			if (bufferbytes == 0)
			{
				break;
			}
		}

		char* p = (char*)buffer_ + readindex_;
		while (*p != '\r' && *p != '\n' && p <= (char*)buffer_ + readindex_)
		{
			raw.append(p, 1);
			readindex_++;
			if (readlength_ - readindex_ == 0)
			{
				break;
			}

			p++;
		}

		if (*p == '\r' || *p == '\n')
		{
			eol = TRUE;
			readindex_++;
			if (*p == '\r' && (readlength_ - readindex_ > 0) && *(p + 1) == '\n')
			{
				readindex_++;
			}
		}
	}

	if (raw.size() > 0)
	{
		if (encoding_ == Encoding::UTF8)
		{
			DWORD cchWideChar = MultiByteToWideChar(CP_UTF8, 0, raw.c_str(), -1, NULL, 0);
			WCHAR* wideChars = (WCHAR*)HeapAlloc(GetProcessHeap(), 0, cchWideChar * sizeof(WCHAR));
			cchWideChar = MultiByteToWideChar(CP_UTF8, 0, raw.c_str(), -1, wideChars, cchWideChar);
			std::wstring line(wideChars);
			HeapFree(GetProcessHeap(), 0, wideChars);
			return line;
		}
		else
		{
			throw std::runtime_error("StreamLineReader::ReadLine() error: the selected encoding is not supported.");
		}
	}

	return TEXT("");
}
