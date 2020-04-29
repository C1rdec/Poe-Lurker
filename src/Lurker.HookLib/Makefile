CC = g++

SYS = ${MSYSTEM}
DUMPMACHINE = ${shell ${CC} -dumpmachine}

WARNS = -Wall

ifeq (${DUMPMACHINE},x86_64-w64-mingw32) 
	DLL = Lurker.HookLib.x64.dll
else
	DLL = Lurker.HookLib.x86.dll
endif

OBJECTS = obj/Source.o obj/FileStream.o

ifeq (${DUMPMACHINE},x86_64-w64-mingw32) 
	CFLAGS = -m64 -std=c++17 -I../Lurker.Common -I../../3rd/asio/asio/include -DWINVER=0x0603 -D_WIN32_WINNT=0x0603 -DUNICODE -D_UNICODE -D_FORTIFY_SOURCE=2 -D_GLIBCXX_ASSERTIONS -DASIO_STANDALONE -O2 ${WARNS} -fmessage-length=0 -fasynchronous-unwind-tables
else
	CFLAGS = -std=c++17 -I../Lurker.Common -I../../3rd/asio/asio/include -DWINVER=0x0603 -D_WIN32_WINNT=0x0603 -DUNICODE -D_UNICODE -D_FORTIFY_SOURCE=2 -D_GLIBCXX_ASSERTIONS -DASIO_STANDALONE -O2 ${WARNS} -fmessage-length=0 -fasynchronous-unwind-tables
endif

LDFLAGS = -s -shared -static -Wl,--subsystem,windows -lws2_32

.PHONY: all clean

all: bin/${DLL}

clean:
ifeq (${SYS}, MINGW64)
	@if [ -d "bin" ]; then rm -r bin; fi
	@if [ -d "obj" ]; then rm -r obj; fi
else
	@if exist bin\* del /f /s /q bin 1>nul & rd /s /q bin
	@if exist obj\* del /f /s /q obj 1>nul & rd /s /q obj
endif

bin obj:
ifeq (${SYS}, MINGW64)
	@if [ ! -d "bin" ]; then mkdir bin; fi
	@if [ ! -d "obj" ]; then mkdir obj; fi
else
	@if not exist "$@" mkdir "$@"
endif

obj/FileStream.o: ../Lurker.Common/FileStream.cpp | obj
	${CC} ${CFLAGS} -c ../Lurker.Common/FileStream.cpp -o obj/FileStream.o

obj/%.o: %.cpp | obj
	${CC} ${CFLAGS} -c "$<" -o "$@"

bin/${DLL}: ${OBJECTS} | bin
	${CC} -o "$@" ${OBJECTS} ${LDFLAGS}

