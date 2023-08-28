import os
import argparse
import subprocess
import glob
import timeit


TEST_CODE_FILE = "in.h"
TEST_STDOUT_FILE = "stdout"
TEST_STDERR_FILE = "stderr"


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--binary", help="Interpreter Binary", required=True)
    parser.add_argument("--tests", help="Tests Directory", required=True)

    args = parser.parse_args()

    tests_location = os.path.abspath(args.tests)

    tests = sorted(glob.glob(os.path.join(tests_location, "*", TEST_CODE_FILE)))

    print(f"\nFound {len(tests)} tests in '{tests_location}' location.\n")

    for test_input_code_file in tests:

        test_dir = os.path.abspath(os.path.dirname(test_input_code_file))
        test_name = os.path.basename(os.path.dirname(test_input_code_file))

        result = subprocess.run(
            [args.binary, test_input_code_file],
            capture_output=True,
            text=True
        )

        if result.returncode == 0:
            expected_output = open(os.path.join(test_dir, TEST_STDOUT_FILE)).read()
            current_output = result.stdout.strip()
        else:
            expected_output = open(os.path.join(test_dir, TEST_STDERR_FILE)).read()
            current_output = result.stderr.strip()

        print(f"{test_name} ->", '\033[92mPASSED' if expected_output == current_output else '\033[91mFAILED', '\033[0m')


if __name__ == "__main__":
    start_time = timeit.default_timer()
    main()
    print(f"\nExeuction time: {(timeit.default_timer() - start_time):.4} seconds.\n")
    