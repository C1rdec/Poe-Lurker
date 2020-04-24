#pragma once

#include "FileStream.h"

#include <string>

#include <Windows.h>

class StreamLineWriter
{
public:
    static constexpr auto kEndOfLine = "\r\n";
public:
    enum class Encoding
    {
        UTF8
    };
    StreamLineWriter(const std::wstring& filepath, bool append)
        : StreamLineWriter(filepath, Encoding::UTF8, append) { }
    StreamLineWriter(const std::wstring& filepath, Encoding encoding, bool append)
        : filestream_(FileStream(filepath, append
            ? FileStream::Mode::Append
            : FileStream::Mode::Truncate)), encoding_(encoding) { }
    //StreamLineWriter(const StreamLineWriter&) = delete;
    //StreamLineWriter(StreamLineWriter&& other)
    //    : filestream_(std::move(other.filestream_)), encoding_(other.encoding_) { }
    //StreamLineWriter& operator=(StreamLineWriter&& other)
    //{
    //    if (this != &other)
    //    {
    //        Close();

    //        filestream_ = std::move(other.filestream_);
    //        encoding_ = other.encoding_;
    //    }

    //    return *this;
    //}
    ~StreamLineWriter()
    {
        Close();
    }
    BOOL autoflush() const { return autoflush_; };
    void set_autoflush(BOOL autoflush) { autoflush_ = autoflush; };
    void Write(const std::wstring& line);
    void WriteLine(const std::wstring& line);
    void Close()
    {
        filestream_.Close();
    }
private:
    FileStream filestream_;
    Encoding encoding_;
    BOOL autoflush_{};
private:
    void WriteEOL()
    {
        filestream_.Write(reinterpret_cast<PBYTE>(const_cast<char*>(kEndOfLine)), 2);
    }
};

inline void StreamLineWriter::Write(const std::wstring& line)
{
    if (line.size() > 0)
    {
        if (encoding_ == Encoding::UTF8)
        {
            auto cbMultiByte = WideCharToMultiByte(CP_UTF8, 0, line.c_str(), -1, NULL, 0, NULL, NULL);
            auto bytes = reinterpret_cast<LPSTR>(HeapAlloc(GetProcessHeap(), 0, cbMultiByte));
            cbMultiByte = WideCharToMultiByte(CP_UTF8, 0, line.c_str(), -1, bytes, cbMultiByte, NULL, NULL);
            filestream_.Write(reinterpret_cast<PBYTE>(bytes), cbMultiByte - 1);
            HeapFree(GetProcessHeap(), 0, bytes);
        }
        else
        {
            throw std::runtime_error("StreamLineWriter::Write() error: the selected encoding is not supported.");
        }
    }
}

inline void StreamLineWriter::WriteLine(const std::wstring& line)
{
    Write(line);
    WriteEOL();
    if (autoflush_)
    {
        filestream_.Flush();
    }
}