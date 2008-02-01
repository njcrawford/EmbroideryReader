#!/bin/sh
convert -geometry 16x16 icon-source.png icon-16.png
convert -geometry 32x32 icon-source.png icon-32.png
convert -geometry 48x48 icon-source.png icon-48.png
icotool -c -o ./icon-all.ico icon-16.png icon-32.png icon-48.png
