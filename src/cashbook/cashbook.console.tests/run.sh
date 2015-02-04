#!/bin/bash
mono cashbook.console.tests.exe deposit -d:1.1.2014 -a:1000
mono cashbook.console.tests.exe deposit -d:1.1.2014 -a:-1000 -desc:"causes error"
mono cashbook.console.tests.exe withdraw -d:2.1.2014 -a:-15 -desc:taxi
mono cashbook.console.tests.exe withdraw -d:2.1.2014 -a:15 -desc:"causes error"
mono cashbook.console.tests.exe sheet -m:1.2.2014
mono cashbook.console.tests.exe export -f:1.2.2014 -t:2.3.2014